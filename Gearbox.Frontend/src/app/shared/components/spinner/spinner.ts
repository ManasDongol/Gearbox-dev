import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [],
  templateUrl: './spinner.html',
 styleUrls: ['./spinner.css'],
})
export class Spinner implements OnInit {
  @ViewChild('gearSvg', { static: true }) gearSvgRef!: ElementRef<SVGElement>;

  ngOnInit() {
    this.buildGear(this.gearSvgRef.nativeElement);
  }

  private buildGear(svgEl: SVGElement) {
    const teeth = 8;
    const outerR = 0.95;
    const innerR = 0.68;
    const toothW = 0.22;
    const toothWO = 0.13;
    const holeR = 0.38;
    const ns = 'http://www.w3.org/2000/svg';
    const px = (r: number, a: number): [number, number] => [Math.cos(a) * r, Math.sin(a) * r];

    let d = '';
    for (let i = 0; i < teeth; i++) {
      const a = (i / teeth) * Math.PI * 2;
      const next = ((i + 1) / teeth) * Math.PI * 2;
      const mid = a + Math.PI / teeth;

      const [x1, y1] = px(innerR, a + toothW);
      const [x2, y2] = px(outerR, mid - toothWO);
      const [x3, y3] = px(outerR, mid + toothWO);
      const [x4, y4] = px(innerR, next - toothW);
      const [nx, ny] = px(innerR, next + toothW);

      d += `${i === 0 ? 'M' : 'L'} ${x1} ${y1} L ${x2} ${y2} L ${x3} ${y3} L ${x4} ${y4} A ${innerR} ${innerR} 0 0 1 ${nx} ${ny} `;
    }
    d += 'Z';

    const path = document.createElementNS(ns, 'path');
    path.setAttribute('d', d);
    path.setAttribute('fill', '#e8f0f0');
    svgEl.appendChild(path);

    const hole = document.createElementNS(ns, 'circle');
    hole.setAttribute('cx', '0');
    hole.setAttribute('cy', '0');
    hole.setAttribute('r', String(holeR));
    hole.setAttribute('fill', 'rgba(26, 58, 58, 0.5)');
    svgEl.appendChild(hole);
  }
}