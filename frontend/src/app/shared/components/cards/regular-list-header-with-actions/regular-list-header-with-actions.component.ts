import { Component, input, output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-regular-list-header-with-actions',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './regular-list-header-with-actions.component.html',
  styleUrl: './regular-list-header-with-actions.component.less',
})
export class RegularListHeaderWithActionsComponent {
  title         = input<string>('');
  icon          = input<string>('');
  color         = input<string>('primary');
  selectedCount = input<number>(0);

  add            = output<void>();
  refresh        = output<void>();
  deleteSelected = output<void>();
}
