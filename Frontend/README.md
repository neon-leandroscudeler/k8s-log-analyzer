# K8s Log Analyzer - Frontend

Frontend em Angular 19 para visualização de logs do Kubernetes.

## Pré-requisitos

- Node.js 20+ 
- npm ou yarn
- Angular CLI 19

## Instalação

```bash
cd Frontend
npm install
```

## Executar o Projeto

```bash
npm start
```

A aplicação estará disponível em: `http://localhost:4200`

## Funcionalidades

### Filtros de Busca
- **Namespace**: Nome do namespace do Kubernetes
- **Pod Name**: Nome do pod para consultar logs

### Data Table
- **Ordenação**: Clique nos cabeçalhos das colunas para ordenar
- **Paginação**: Navegue entre páginas com 10, 25, 50 ou 100 itens por página
- **Filtro em Tempo Real**: Digite no campo de busca para filtrar logs em todas as colunas

### Diferenciação Visual
- **ERROR**: Chip vermelho
- **WARN**: Chip laranja
- **INFO**: Chip azul
- **DEBUG**: Chip cinza

## Estrutura do Projeto

```
src/
├── app/
│   ├── components/
│   │   └── log-viewer/          # Componente principal
│   ├── models/                   # Interfaces TypeScript
│   ├── services/                 # Serviços HTTP
│   ├── app.component.ts          # Componente raiz
│   └── app.config.ts             # Configuração da aplicação
├── index.html                    # HTML principal
├── main.ts                       # Bootstrap da aplicação
└── styles.scss                   # Estilos globais
```

## Build para Produção

```bash
npm run build
```

Os arquivos otimizados estarão na pasta `dist/`.

## Tecnologias Utilizadas

- Angular 19 (Standalone Components)
- Angular Material
- RxJS
- TypeScript
