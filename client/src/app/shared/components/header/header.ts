import { Component, computed, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../material.imports';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterModule, ...MATERIAL_IMPORTS],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  mobileMenuOpen = signal(false);
  menuIcon = computed(() => this.mobileMenuOpen() ? 'close' : 'menu');

  onMenuToggle(): void {
    this.mobileMenuOpen.update(open => !open);
  }
}
