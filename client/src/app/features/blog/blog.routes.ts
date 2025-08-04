import { Routes } from '@angular/router';

export const blogRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import('./components/article-list/article-list').then(c => c.ArticleList)
    },
    {
        path: 'article/:id',
        loadComponent: () => import('./components/article-detail/article-detail').then(c => c.ArticleDetail)
    }
];
