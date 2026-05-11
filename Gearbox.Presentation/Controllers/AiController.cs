using System.Security.Claims;
using AgroLink.Domain.Entities;
using Gearbox.Application.DTOs.Ai;
using Gearbox.Domain.Entities;
using Gearbox.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gearbox.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _db;
    private const int CONVERSATION_LIMIT = 10;

    public AiController(HttpClient httpClient, ApplicationDbContext db)
    {
        _httpClient = httpClient;
        _db = db;
    }

    // ─── POST /api/Ai/ask ─────────────────────────────────────
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        AiSession session;

        // ─── Get or create session ───────────────────────────
        if (request.SessionId.HasValue)
        {
            session = await _db.AiSessions
                .Include(s => s.AiMessages)
                .FirstOrDefaultAsync(s => s.Id == request.SessionId && s.UserId == userId);

            if (session == null)
                return NotFound(new { error = "Session not found." });

            var userMessageCount = await _db.AiMessages
                .Where(m => m.AiSessionId == session.Id && m.Role == AiMessageRole.User)
                .CountAsync();

            if (userMessageCount >= CONVERSATION_LIMIT)
                return BadRequest(new
                {
                    error = "Conversation limit reached",
                    message = "Start a new chat."
                });
        }
        else
        {
            var title = request.Query.Length > 40
                ? request.Query[..40] + "..."
                : request.Query;

            session = new AiSession
            {
                UserId = userId,
                Title = title
            };

            _db.AiSessions.Add(session);
        }

        // ─── Save user message ────────────────────────────────
        _db.AiMessages.Add(new AiMessage
        {
            AiSessionId = session.Id,
            Role = AiMessageRole.User,
            Content = request.Query
        });

        // ─── Call Python FastAPI ──────────────────────────────
        var pyResponse = await _httpClient.PostAsJsonAsync(
            "http://localhost:8000/ask",
            new { query = request.Query }
        );

        if (!pyResponse.IsSuccessStatusCode)
            return StatusCode(502, new { error = "AI service unavailable." });

        var pyResult = await pyResponse.Content
            .ReadFromJsonAsync<Dictionary<string, string>>();

        var answer = pyResult?["answer"];

        if (string.IsNullOrEmpty(answer))
            return StatusCode(500, new { error = "Invalid AI response." });

        // ─── Save AI message ──────────────────────────────────
        _db.AiMessages.Add(new AiMessage
        {
            AiSessionId = session.Id,
            Role = AiMessageRole.Ai,
            Content = answer
        });

        session.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var totalUserMessages = await _db.AiMessages
            .Where(m => m.AiSessionId == session.Id && m.Role == AiMessageRole.User)
            .CountAsync();

        return Ok(new AiResponseDto
        {
            Answer = answer,
            SessionId = session.Id,
            SessionTitle = session.Title,
            QueriesUsed = totalUserMessages,
            QueriesRemaining = CONVERSATION_LIMIT - totalUserMessages,
            ConversationEnded = totalUserMessages >= CONVERSATION_LIMIT
        });
    }

    // ─── GET sessions ───────────────────────────────────────
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var sessions = await _db.AiSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => new AiSessionDto
            {
                Id = s.Id,
                Title = s.Title,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();

        return Ok(sessions);
    }

    // ─── GET session messages ───────────────────────────────
    [HttpGet("sessions/{id}")]
    public async Task<IActionResult> GetSession(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var messages = await _db.AiMessages
            .Where(m => m.AiSessionId == id && m.AiSession.UserId == userId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new AiMessageDto
            {
                Role = m.Role == AiMessageRole.User ? "user" : "ai",
                Content = m.Content,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();

        return Ok(messages);
    }

    // ─── DELETE session ──────────────────────────────────────
    [HttpDelete("sessions/{id}")]
    public async Task<IActionResult> DeleteSession(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var session = await _db.AiSessions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (session == null)
            return NotFound();

        _db.AiSessions.Remove(session);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}