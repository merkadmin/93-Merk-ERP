import { Component, input, output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-regular-operation-actions',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './regular-operation-actions.component.html',
  styleUrl: './regular-operation-actions.component.less',
})
export class RegularOperationActionsComponent {
  saving = input<boolean>(false);
  savingNew = input<boolean>(false);

  save = output<void>();
  saveAndNew = output<void>();
  reset = output<void>();
}
