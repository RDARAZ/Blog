import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        redirectTo: '/blog',
        pathMatch: 'full'
    },
    {
        path: 'blog',
        loadChildren: () => import('./features/blog/blog.routes').then(m => m.blogRoutes)
    },
    {
        path: 'auth',
        loadChildren: () => import('./features/auth/auth.routes').then(m => m.authRoutes)
    },
    {
        path: 'admin',
        loadChildren: () => import('./features/admin/admin.routes').then(m => m.adminRoutes)
    },
    {
        path: '**',
        redirectTo: '/blog'
    }
];
