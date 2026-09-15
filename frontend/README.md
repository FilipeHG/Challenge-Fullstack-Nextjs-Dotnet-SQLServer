# 🚀 Support Requests - Frontend

> Dashboard para gerenciamento de solicitações internas de suporte.

## 📋 Overview

Frontend do desafio `Challenge-Fullstack-Nextjs-Dotnet-SQLServer`.
A aplicação consome a API .NET existente e mantém a lógica de negócio no servidor.

| Aplicação | Endereço local                           |
| --------- | ---------------------------------------- |
| Frontend  | http://localhost:3001                    |
| Backend   | http://localhost:3000                    |
| Swagger   | http://localhost:3000/swagger/index.html |
| OpenAPI   | http://localhost:3000/openapi/v1.json    |

## ✨ Funcionalidades

- Listagem com paginação no servidor.
- Criação com validação em português.
- Consulta dos detalhes atualizados por ID.
- Alteração de prioridade e status.
- Exclusão de solicitações abertas com confirmação.
- Filtros por status e prioridade.
- Pesquisa com debounce de 350 ms.
- Estados de carregamento, vazio e erro recuperável.
- Indicador de disponibilidade da API.
- Tabela no desktop e cartões no celular.
- Feedback de operações por notificações.
- Formulários e modais acessíveis por teclado.

## 🛠 Tecnologias Utilizadas

| Tecnologia                 | Versão / finalidade                  |
| -------------------------- | ------------------------------------ |
| Node.js                    | 24+                                  |
| npm                        | Versão distribuída com Node 24       |
| Next.js                    | 16.3.5, Pages Router                 |
| React / React DOM          | 19.2.7                               |
| TypeScript                 | 5.9.3, modo estrito                  |
| Tailwind CSS               | 3.4.19                               |
| TanStack Query             | Cache remoto e mutations             |
| TanStack Table             | Série 8, tabela sem estilos impostos |
| Axios                      | HTTP e interceptor Bearer            |
| React Hook Form / Zod      | Formulário e validação               |
| Radix UI                   | Dialog, AlertDialog e Select         |
| Lucide / React Hot Toast   | Ícones e notificações                |
| Vitest / Testing Library   | Testes unitários e de componentes    |
| Playwright                 | Testes de navegador                  |
| ESLint / Oxlint / Prettier | Qualidade e formatação               |

As versões resolvidas estão registradas no `package-lock.json`.
TanStack Table foi fixado na série 8 para preservar a API da arquitetura de referência.

## 🧭 Decisão de Router

Foi escolhido o Pages Router porque o desafio permite ambas as abordagens
e a aplicação possui um único dashboard orientado a estado no cliente
e consumo de uma API .NET existente.
Essa escolha preserva a arquitetura simples descrita no projeto de referência.

Não há requisito concreto para Server Components, Server Actions ou SEO dinâmico.
`_app.tsx` concentra os providers, layout e notificações.
`pages/index.tsx` representa a entrada do dashboard.

O build não depende da API: as consultas começam no navegador.
Cada instância da aplicação cria seu próprio QueryClient com `useState`.

## 🌳 Git / Branch

Branch de desenvolvimento: `filipe-goncalves`.

```powershell
git branch --show-current
git status --short
```

Nenhum commit ou push é executado automaticamente na entrega.
Arquivos locais com credenciais são excluídos pelo `.gitignore`.
As alterações do backend são preservadas.

## 🔗 Dependência do Backend

Execute o backend conforme as instruções do diretório `backend/`.
A API deve estar disponível na porta 3000.

```powershell
Invoke-RestMethod http://localhost:3000/api/health
```

A saúde da API depende também da disponibilidade do SQL Server.
Falhas no indicador de saúde não produzem notificações repetitivas.
A verificação é refeita a cada 30 segundos.

## ⚙️ Variáveis de Ambiente

```powershell
Copy-Item .env.example .env.local
```

Edite `.env.local`:

```env
NEXT_PUBLIC_APP_NAME=Support Requests
NEXT_PUBLIC_API_BASE_URL=/api
NEXT_PUBLIC_API_JWT_TOKEN=CHANGE_ME
```

| Variável                  | Uso                            |
| ------------------------- | ------------------------------ |
| NEXT_PUBLIC_APP_NAME      | Nome previsto para a aplicação |
| NEXT_PUBLIC_API_BASE_URL  | Base da API, incluindo `/api`  |
| NEXT_PUBLIC_API_JWT_TOKEN | Token fornecido para o desafio |

Substitua `CHANGE_ME` pelo token local, sem o prefixo `Bearer `.
Reinicie o servidor de desenvolvimento após alterar as variáveis.
As variáveis públicas são incorporadas ao bundle de produção durante o build.

## 🔐 Autenticação

O backend do desafio não oferece fluxo de login.
O frontend utiliza um Bearer token previamente gerado em `.env.local`.
O interceptor adiciona o cabeçalho às chamadas protegidas.
O endpoint público de saúde não recebe esse cabeçalho.

O token está visível ao JavaScript do navegador por ser uma variável pública.
Essa escolha é específica do desafio e não constitui autenticação de produção.
Uma aplicação de produção deve usar login, sessões e credenciais de curta duração.

Não há refresh de token nem armazenamento em localStorage.
Não configure chaves de assinatura, senhas de banco ou credenciais SQL no frontend.
Nunca publique `.env.local` ou o bootstrap local.

## 📦 Instalação

Pré-requisitos:

1. Node.js 24 ou superior.
2. npm disponível no terminal.
3. Backend iniciado para uso integrado.

No diretório `frontend`:

```powershell
npm ci
npx playwright install chromium
```

Use `npm install` ao alterar dependências intencionalmente.
Versione o lockfile junto das alterações de dependência.

## ▶️ Executando Localmente

```powershell
npm run dev
```

Abra http://localhost:3001.

Para acesso por outro dispositivo na rede:

```powershell
npm run dev:host
```

Nesse caso, configure uma URL de API acessível pelo dispositivo.
O backend também precisa permitir a origem utilizada.

## 🏗 Build de Produção

```powershell
npm run build
npm start
```

A aplicação de produção usa a porta 3001.
Não é necessário manter o backend ativo durante o build.
A pasta `.next/` contém o resultado e é ignorada pelo Git.

## 📜 Scripts Disponíveis

| Comando              | Finalidade                         |
| -------------------- | ---------------------------------- |
| npm run dev          | Desenvolvimento na porta 3001      |
| npm run dev:host     | Desenvolvimento na rede local      |
| npm run build        | Build de produção                  |
| npm start            | Servir o build na porta 3001       |
| npm run typecheck    | Verificação TypeScript             |
| npm run lint         | Oxlint e ESLint                    |
| npm run format       | Aplicar Prettier                   |
| npm run format:check | Conferir formatação                |
| npm test             | Testes unitários e componentes     |
| npm run test:watch   | Vitest em modo watch               |
| npm run test:cov     | Relatório de cobertura             |
| npm run test:e2e     | Playwright desktop e mobile        |
| npm run test:e2e:ui  | Interface interativa do Playwright |
| npm run verify       | Tipos, lint, testes e build        |

## 🏛 Arquitetura

```text
Page
  ↓
Components
  ↓
Hooks
  ↓
TanStack Query
  ↓
Services
  ↓
Axios
  ↓
.NET API
```

A organização segue a arquitetura especificada de `ProjectValidatorSystem-Frontend`.
Não foi localizada uma cópia desse projeto no diretório de repositórios inspecionado.
A referência visual foi aplicada conforme o bootstrap: slate, azul/indigo e cartões claros.

Componentes não fazem chamadas Axios diretamente.
Services normalizam o transporte e não exibem notificações.
Hooks controlam consultas, mutations, feedback e invalidação.
Schemas concentram as regras do formulário.

## 📂 Estrutura do Projeto

```text
frontend/
├── public/favicon.svg
├── src/
│   ├── components/
│   │   ├── cards/
│   │   ├── dialogs/
│   │   ├── filters/
│   │   ├── modals/
│   │   ├── states/
│   │   └── tables/
│   ├── hooks/
│   ├── layouts/
│   ├── pages/
│   ├── schemas/
│   ├── services/
│   ├── styles/
│   ├── types/
│   └── utils/
├── tests/
│   ├── components/
│   ├── e2e/
│   └── unit/
├── .env.example
├── package.json
├── package-lock.json
└── AI_USAGE.md
```

## 🔄 Fluxo de Dados

A listagem usa a chave `['supportRequests', query]`.
A chave inclui página, limite, status, prioridade e pesquisa.
Os detalhes usam `['supportRequest', id]` e são atualizados ao abrir o modal.

Configuração padrão de consultas:

- `staleTime`: 30 segundos.
- `retry`: uma tentativa adicional.
- `refetchOnWindowFocus`: desativado.

Após uma mutation bem-sucedida, as listagens são invalidadas.
Alterações de prioridade/status também invalidam os detalhes correspondentes.
Não há atualização otimista: o estado retornado pelo servidor é a autoridade.
Isso evita lógica de rollback para transições que podem ser recusadas.

Filtros e tamanho de página reiniciam a paginação em 1.
A pesquisa é normalizada com trim e enviada após 350 ms.
A pesquisa não filtra apenas os registros já carregados.

## 🌐 Integração com API

| Método | Rota                            | Operação           |
| ------ | ------------------------------- | ------------------ |
| GET    | /health                         | Disponibilidade    |
| GET    | /support-requests               | Listar             |
| POST   | /support-requests               | Criar              |
| GET    | /support-requests/{id}          | Consultar detalhes |
| PATCH  | /support-requests/{id}/priority | Alterar prioridade |
| PATCH  | /support-requests/{id}/status   | Alterar status     |
| DELETE | /support-requests/{id}          | Excluir aberta     |

Todas as rotas acima são relativas à base `/api`.
Timeout das chamadas: 15 segundos.

Prioridades: `low`, `medium`, `high`.
Status: `open`, `inProgress`, `completed`.
O mapper aceita os enums numéricos 1/2/3 e os equivalentes textuais.
Componentes recebem apenas os valores normalizados.

Criação envia título, descrição, solicitante e prioridade.
ID, status e datas são controlados pelo backend.
Título e solicitante têm limite de 200 caracteres; descrição, 4.000.
Campos contendo apenas espaços são rejeitados.

Solicitações concluídas podem voltar para em andamento, mas não para aberta.
Somente solicitações abertas exibem exclusão habilitada.
Conflitos concorrentes continuam sendo validados pelo backend.

Problem Details é convertido em mensagem: detalhe, erros de validação, título ou fallback.
Objetos de erro completos, cabeçalhos e stack traces não são exibidos.

## 🧪 Testes

```powershell
npm run typecheck
npm run lint
npm test
npm run build
npm run test:e2e
npm run format:check
```

Testes unitários cobrem validação, enums, mensagens de erro e interceptor Bearer.
O teste de autenticação utiliza somente `test-token`.

Os testes de componentes cobrem criação, validação, refetch, erro recuperável e debounce.
Os testes E2E interceptam `/api/**` e não dependem da API real.
Os cenários incluem criação, filtros, detalhes, alteração de status, exclusão e layout.

Viewports de referência:

- Desktop: 1440 × 900.
- Mobile: 375 × 812.

Relatórios ficam em `playwright-report/` e `test-results/`, ignorados pelo Git.
Traces ficam desabilitados para evitar persistir cabeçalhos de autenticação.
Capturas de tela mostram apenas a interface com dados simulados.

## ♿ Responsividade e Acessibilidade

Abaixo de 768 px, a listagem utiliza cartões.
No desktop, utiliza tabela semântica com cabeçalhos.
Filtros são empilhados no celular e organizados em linha no desktop.
Modais respeitam a altura da tela e permitem rolagem interna.

Radix gerencia foco, navegação de selects e fechamento por Escape.
Campos têm labels e erros vinculados por atributos acessíveis.
Botões de ícone têm nomes acessíveis e estados de foco visíveis.
Status e prioridade incluem texto, além das cores.
Há um link para saltar ao conteúdo principal.

## 🧠 Decisões Técnicas

- Pages Router adequado ao dashboard cliente.
- React Hook Form para estado de formulário.
- TanStack Query para estado remoto.
- Estado local React para filtros e modais.
- Invalidação explícita após mutations.
- Normalização de enums na fronteira de transporte.
- Rewrite Next.js para resolver o CORS confirmado na API local.
- Datas apresentadas com Intl em português do Brasil.

## ⚠️ Limitações

- JWT fixo e público no runtime, específico do desafio.
- Não há login, renovação de sessão ou autorização por perfil no frontend.
- Não há atualizações otimistas.
- A semântica de pesquisa depende do backend.
- Somente prioridade e status são editáveis pelo contrato atual.
- Comunicação entre portas exige CORS do backend para localhost:3001.
- Não foi solicitado deploy de produção.
- Não foi realizada auditoria formal completa de acessibilidade.

## 🚀 Melhorias Futuras

- Autenticação real com credenciais de curta duração.
- Autorização por perfis.
- Filtros sincronizados com a URL.
- Preferência persistida de tamanho de página.
- Telemetria com tratamento de dados sensíveis.
- Pipeline de CI e deploy dos dois projetos.
- Auditoria de acessibilidade com tecnologias assistivas.
- Cliente gerado do OpenAPI se o contrato crescer.

## ⏱ Tempo de Desenvolvimento

Tempo aproximado utilizado: 8 horas

Estimativa informada pelo desenvolvedor no bootstrap; não é uma medição desta execução.

## 🤖 Uso de IA

OpenAI Codex foi usado como apoio à análise de requisitos, arquitetura,
implementação, geração de testes, depuração e documentação.
O contrato OpenAPI e o código do backend orientaram a integração.

As decisões arquiteturais foram fornecidas pelo desenvolvedor no bootstrap.
A execução automatizada verifica tipos, lint, testes e build.
A revisão humana de todas as alterações deve ser confirmada pelo desenvolvedor;
a automação não atesta uma revisão humana que não observou.

Consulte `AI_USAGE.md` para o registro do processo e verificações.

## 🔀 Ajuste de CORS verificado

A comunicação direta de localhost:3001 para localhost:3000 foi testada no navegador e recusada por ausência de Access-Control-Allow-Origin, inclusive no preflight da listagem. O frontend utiliza NEXT_PUBLIC_API_BASE_URL=/api e um rewrite do Next.js para http://localhost:3000/api. O backend permanece inalterado. O Bearer continua sendo enviado pelo cliente; o rewrite não implementa autenticação nem regras de negócio.

O destino pode ser configurado com API_PROXY_TARGET (variável apenas do servidor, sem barra final). Caso o backend passe a permitir CORS, a base pública pode voltar a ser sua URL absoluta. Reinicie o Next.js após mudar a configuração.

## ✅ Verificação da entrega

26 testes unitários/de componentes e 14 testes E2E aprovados. Typecheck, lint, formatação e build aprovados. Integração real validada pelo navegador, com remoção do registro temporário. Veja IMPLEMENTATION_REPORT.md para o relatório completo.

React Query Devtools pode ser habilitado em desenvolvimento com NEXT_PUBLIC_QUERY_DEVTOOLS=true.
