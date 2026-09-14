# Challenge técnico Fullstack

## React/Next.js, C#/.NET e SQL Server

### Objetivo
#### Crie uma branch com seu nome e realize os commtis na sua branch.

Desenvolver uma aplicação web para registrar, acompanhar e concluir solicitações internas. O desafio busca avaliar domínio da stack, organização do código, modelagem de dados, decisões técnicas e capacidade de explicar a solução.

**Tempo sugerido:** 6 a 8 horas. Não esperamos uma aplicação com acabamento de produção. Caso o tempo termine, documente no README o que ficou pendente e como seria implementado.

## Cenário

Uma empresa precisa controlar solicitações internas de suporte. Cada solicitação deve possuir:

- título;
- descrição;
- solicitante;
- prioridade: baixa, média ou alta;
- status: aberta, em andamento ou concluída;
- data de criação;
- data de conclusão, quando aplicável.

## Requisitos obrigatórios

### Front-end

Utilize **React com Next.js e TypeScript**.

A aplicação deve permitir:

1. listar as solicitações;
2. cadastrar uma solicitação;
3. visualizar os detalhes;
4. alterar prioridade e status;
5. filtrar por status e prioridade;
6. pesquisar pelo título ou solicitante;
7. paginar os resultados;
8. apresentar estados de carregamento, lista vazia e erro;
9. validar o formulário e exibir mensagens compreensíveis ao usuário;
10. funcionar adequadamente em telas de computador e celular.

O candidato pode escolher App Router ou Pages Router, desde que explique a decisão.

### Back-end

Utilize **C# com ASP.NET Core Web API** e uma versão do .NET com suporte ativo.

A API deve oferecer operações para:

- cadastrar uma solicitação;
- consultar uma solicitação por identificador;
- listar solicitações com filtros, pesquisa e paginação;
- atualizar prioridade e status;
- excluir uma solicitação ainda aberta.

Implemente as seguintes regras:

1. título, descrição e solicitante são obrigatórios;
2. uma solicitação concluída deve receber a data de conclusão automaticamente;
3. uma solicitação concluída não pode voltar para aberta;
4. apenas solicitações abertas podem ser excluídas;
5. valores inválidos devem gerar resposta HTTP adequada e mensagem clara;
6. registros inexistentes devem retornar `404 Not Found`.

Organize as responsabilidades de modo que regras de negócio não fiquem concentradas nos controllers.

### Banco de dados

Utilize **Microsoft SQL Server**.

A solução deve incluir:

- modelo relacional coerente;
- chave primária e restrições necessárias;
- migrations ou script SQL versionado para criação do banco;
- persistência utilizando Entity Framework Core ou outra abordagem justificada;
- consultas parametrizadas;
- ordenação padrão pelas solicitações mais recentes;
- pelo menos um índice criado para apoiar os filtros ou a pesquisa implementada.

No README, explique a finalidade do índice escolhido e seu impacto esperado.

### Testes

Inclua testes automatizados relevantes. O mínimo esperado é:

- dois testes de regras de negócio no back-end;
- um teste de endpoint ou integração no back-end;
- um teste de componente ou fluxo principal no front-end.

Priorizamos a qualidade e a relevância dos cenários sobre a quantidade de testes.

## Diferenciais opcionais

Os itens abaixo não são obrigatórios e não devem comprometer os requisitos principais:

- autenticação e autorização;
- atualização otimista ou estratégia explícita de cache;
- Docker Compose para aplicação e SQL Server;
- documentação OpenAPI/Swagger aprimorada;
- acessibilidade demonstrável;
- observabilidade, logs estruturados ou correlação de requisições;
- pipeline de integração contínua;
- publicação em ambiente acessível.

## Entrega

Disponibilize o código em um repositório Git e inclua um `README.md` contendo:

1. instruções completas para executar a solução;
2. versões e dependências necessárias;
3. forma de criar e configurar o banco;
4. decisões técnicas e justificativas;
5. limitações conhecidas;
6. melhorias que faria com mais tempo;
7. tempo aproximado utilizado;
8. indicação de ferramentas de IA utilizadas e como foram usadas.

Não inclua senhas ou outros segredos no repositório. Forneça arquivos de exemplo para as configurações necessárias.
