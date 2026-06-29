import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () =>
            import('./pages/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: '',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./layout/admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
        children: [
            {
                path: 'dashboard',
                loadComponent: () =>
                    import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent)
            },
            {
                path: 'clientes',
                loadComponent: () =>
                    import('./pages/clientes/clientes.component').then(m => m.ClientesComponent)
            },
            {
                path: 'produtos',
                loadComponent: () =>
                    import('./pages/produtos/produtos.component').then(m => m.ProdutosComponent)
            },
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'dashboard'
            },
        ]
    },
    {
        path: '**',
        redirectTo: 'dashboard'
    }
];