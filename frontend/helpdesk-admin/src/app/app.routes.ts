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
                path: 'produto-x-cliente',
                loadComponent: () =>
                    import('./pages/produto-x-cliente/produto-x-cliente.component').then(m => m.ProdutoXClienteComponent)
            },
            {
                path: 'tipo-ocorrencia',
                loadComponent: () =>
                    import('./pages/tipo-ocorrencia/tipo-ocorrencia.component').then(m => m.TipoOcorrenciaComponent)
            },
            {
                path: 'status-ticket',
                loadComponent: () =>
                    import('./pages/status-ticket/status-ticket.component').then(m => m.StatusTicketComponent)
            },
            {
                path: 'tecnicos',
                loadComponent: () =>
                    import('./pages/tecnicos/tecnicos.component').then(m => m.TecnicosComponent)
            },
            {
                path: 'tickets',
                loadComponent: () =>
                    import('./pages/tickets/tickets.component').then(m => m.TicketsComponent)
            },
            {
                path: 'usuarios',
                loadComponent: () =>
                    import('./pages/usuarios/usuarios.component').then(m => m.UsuariosComponent)
            },
            {
                path: 'minha-conta',
                loadComponent: () =>
                    import('./pages/minha-conta/minha-conta.component').then(m => m.MinhaContaComponent)
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