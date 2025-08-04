import { Routes } from '@angular/router';

export const adminRoutes: Routes = [
  {
    path: 'dashboard',
    loadComponent: () => import('./components/dashboard/dashboard/dashboard').then(c => c.Dashboard)
  },
  {
    path: 'articles/new',
    loadComponent: () => import('./components/article-editor/article-editor/article-editor').then(c => c.ArticleEditor)
  },
  {
    path: 'articles/edit/:id',
    loadComponent: () => import('./components/article-editor/article-editor/article-editor').then(c => c.ArticleEditor)
  }
];