# Uso de IA

## Processo

OpenAI Codex executou a implementação a partir de `PROJECT_BOOTSTRAP.md`, uma especificação local fornecida pelo desenvolvedor.
A arquitetura Pages → Components → Hooks → TanStack Query → Services → Axios foi definida nessa especificação.
Foram inspecionados o OpenAPI ativo e o controller da API .NET para confirmar as rotas e os contratos.

O Codex auxiliou na análise de requisitos, implementação, cenários de teste, depuração e documentação.
Não há evidência nesta execução de uso do Antigravity; por isso ele não é declarado como ferramenta utilizada.
Não foram usados subagentes.

## Decisões humanas e revisão

O desenvolvedor forneceu as decisões de stack, router, arquitetura, domínio e restrições no bootstrap.
Não é possível afirmar que todas as alterações geradas passaram por revisão humana sem confirmação do desenvolvedor.
As verificações automatizadas são evidência de execução, não substituem essa revisão.

## Proteção de credenciais

O token fornecido foi transferido diretamente do bootstrap para `.env.local`, sem inserção em código ou fixtures.
Esses dois arquivos são ignorados pelo Git.
Testes do interceptor usam um token fictício.
Traces do Playwright são desabilitados para não registrar cabeçalhos.
Nenhum segredo do backend foi necessário à implementação.

## Verificação

O projeto inclui verificação TypeScript, Oxlint/ESLint, Prettier, Vitest, build Next.js e Playwright.
Os testes de navegador simulam a API, incluindo mutations e o estado de conclusão.
A integração real deve ser distinguida desses testes determinísticos no relatório final.

## Ajuste de compatibilidade

A instalação inicial de TanStack Table mais recente selecionou uma API incompatível com os hooks da série 8 previstos na implementação.
A dependência foi fixada na série 8 e a verificação repetida.

## Escopo

Nenhum backend, migration, segredo de assinatura ou regra de negócio foi alterado.
Nenhum commit ou push foi realizado.
A duração de 8 horas é a estimativa fornecida pelo desenvolvedor, não uma medição da execução automatizada.

## Integração real e CORS

O navegador confirmou falta de Access-Control-Allow-Origin na API. Foi aplicado o fallback de rewrite autorizado no bootstrap, restrito ao destino configurado, sem alterações no backend.

## Resultado final

26 testes Vitest e 14 cenários Playwright aprovados. Typecheck, lint, formatação e build aprovados. O ciclo real de criação, consulta, atualização e exclusão foi validado pelo navegador após o rewrite. O registro temporário foi removido. Consulte IMPLEMENTATION_REPORT.md.
