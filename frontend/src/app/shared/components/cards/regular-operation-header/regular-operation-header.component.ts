import { Component, input, output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-regular-operation-header',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './regular-operation-header.component.html',
  styleUrl: './regular-operation-header.component.less',
})
export class RegularOperationHeaderComponent {
  title = input.required<string>();
  icon = input.required<string>();
  color = input<string>('primary');

  cancel = output<void>();
}
