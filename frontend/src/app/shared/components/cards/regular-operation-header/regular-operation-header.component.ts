import { Component, input } from '@angular/core';

@Component({
  selector: 'app-regular-operation-header',
  standalone: true,
  templateUrl: './regular-operation-header.component.html',
})
export class RegularOperationHeaderComponent {
  title = input.required<string>();
  icon  = input.required<string>();
  color = input<string>('primary');
}
