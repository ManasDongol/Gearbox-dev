namespace Gearbox.Application.DTOs;


    public class Result
    {
        public bool IsSuccess { get; private set; }
        public IEnumerable<string>? Errors { get; private set; }

        public static Result Success() => new() { IsSuccess = true };
        public static Result Failure(IEnumerable<string> errors) => new() { IsSuccess = false, Errors = errors };
        public static Result Failure(string error) => new() { IsSuccess = false, Errors = new List<string> { error } };
    }
