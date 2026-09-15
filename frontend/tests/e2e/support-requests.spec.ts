import { test, expect } from "@playwright/test";
import { request as fixture, paged } from "../fixtures";
test.beforeEach(async ({ page }) => {
  let item = { ...fixture };
  await page.route("**/api/**", async (route) => {
    const req = route.request();
    const url = new URL(req.url());
    let data: unknown = { status: "Healthy" };
    if (url.pathname.endsWith("/support-requests")) {
      if (req.method() === "POST") {
        item = { ...item, ...req.postDataJSON(), id: 2 };
        return route.fulfill({ status: 201, json: item });
      }
      data = { ...paged, items: [item] };
    } else if (url.pathname.includes("/support-requests/")) {
      if (req.method() === "DELETE") return route.fulfill({ status: 204 });
      if (req.method() === "PATCH") {
        item = { ...item, ...req.postDataJSON() };
        item.completedAt =
          item.status === "completed" ? "2026-09-14T13:00:00Z" : null;
      }
      data = item;
    }
    await route.fulfill({ json: data });
  });
  await page.goto("/");
});
test("carrega dashboard e cria com validação", async ({ page }) => {
  await expect(
    page.getByRole("heading", { name: "Solicitações de Suporte" }),
  ).toBeVisible();
  await expect(page.getByText(fixture.title).first()).toBeAttached();
  await page.getByRole("button", { name: "Nova Solicitação" }).click();
  const modal = page.getByRole("dialog");
  await expect(modal).toBeVisible();
  await modal.getByRole("button", { name: "Criar Solicitação" }).click();
  await expect(modal.getByText("Título é obrigatório.")).toBeVisible();
  await modal.getByLabel(/^Título/).fill("Rede indisponível");
  await modal.getByLabel(/^Solicitante/).fill("Ana");
  await modal.getByLabel(/^Descrição/).fill("Sem acesso à rede interna.");
  await modal.getByRole("button", { name: "Criar Solicitação" }).click();
  await expect(modal).toBeHidden();
  await expect(page.getByText("Solicitação criada com sucesso.")).toBeVisible();
});
test("envia filtros e busca para a API", async ({ page }) => {
  await page.getByRole("combobox", { name: "Status", exact: true }).click();
  const statusRequest = page.waitForRequest((req) =>
    req.url().includes("status=open"),
  );
  await page.getByRole("option", { name: "Aberta", exact: true }).click();
  await statusRequest;
  await page.getByRole("combobox", { name: "Prioridade", exact: true }).click();
  const priorityRequest = page.waitForRequest((req) =>
    req.url().includes("priority=high"),
  );
  await page.getByRole("option", { name: "Alta", exact: true }).click();
  await priorityRequest;
  const searchRequest = page.waitForRequest(
    (req) => new URL(req.url()).searchParams.get("search") === "Maria",
  );
  await page
    .getByRole("textbox", { name: "Pesquisar por título ou solicitante" })
    .fill(" Maria ");
  await searchRequest;
  await page.getByRole("button", { name: "Limpar filtros" }).click();
  await expect(
    page.getByRole("textbox", { name: "Pesquisar por título ou solicitante" }),
  ).toHaveValue("");
});
test("detalhes atualizam prioridade e restringem status concluído", async ({
  page,
}) => {
  await page
    .locator("button:visible")
    .filter({ has: page.locator("svg.lucide-eye") })
    .click();
  const modal = page.getByRole("dialog");
  await expect(modal.getByText(fixture.description)).toBeVisible();
  await modal.getByRole("combobox", { name: "Alterar prioridade" }).click();
  await page.getByRole("option", { name: "Baixa", exact: true }).click();
  await expect(
    modal.getByRole("combobox", { name: "Alterar prioridade" }),
  ).toHaveText("Baixa");
  await modal.getByRole("combobox", { name: "Alterar status" }).click();
  await page.getByRole("option", { name: "Concluída", exact: true }).click();
  await expect(
    modal.getByRole("combobox", { name: "Alterar status" }),
  ).toHaveText("Concluída");
  await modal.getByRole("combobox", { name: "Alterar status" }).click();
  await expect(
    page.getByRole("option", { name: "Aberta", exact: true }),
  ).toHaveCount(0);
});
test("confirma exclusão e permite cancelar", async ({ page }) => {
  await page
    .locator("button:visible")
    .filter({ has: page.locator("svg.lucide-trash-2") })
    .click();
  await page
    .getByRole("alertdialog")
    .getByRole("button", { name: "Cancelar" })
    .click();
  await expect(page.getByRole("alertdialog")).toBeHidden();
  await page
    .locator("button:visible")
    .filter({ has: page.locator("svg.lucide-trash-2") })
    .click();
  const deletion = page.waitForRequest((req) => req.method() === "DELETE");
  await page
    .getByRole("alertdialog")
    .getByRole("button", { name: "Excluir solicitação", exact: true })
    .click();
  await deletion;
  await expect(page.getByRole("alertdialog")).toBeHidden();
});
test("layout cabe no viewport", async ({ page }, info) => {
  await expect(
    page.getByRole("heading", { name: "Solicitações de Suporte" }),
  ).toBeVisible();
  expect(
    await page.evaluate(
      () => document.documentElement.scrollWidth <= window.innerWidth,
    ),
  ).toBe(true);
  if (info.project.name === "mobile") {
    await expect(page.getByRole("article")).toBeVisible();
    await expect(page.getByRole("table")).toBeHidden();
  } else await expect(page.getByRole("table")).toBeVisible();
  await page.screenshot({
    path: `test-results/dashboard-${info.project.name}.png`,
    fullPage: true,
  });
});

test("paginação respeita os flags e reinicia ao filtrar", async ({ page }) => {
  await page.route("**/api/support-requests?**", async (route) => {
    const url = new URL(route.request().url());
    const current = Number(url.searchParams.get("page"));
    await route.fulfill({
      json: {
        ...paged,
        page: current,
        totalItems: 11,
        totalPages: 2,
        hasNextPage: current === 1,
        hasPreviousPage: current === 2,
      },
    });
  });
  await page.reload();
  await expect(
    page.getByRole("button", { name: "Anterior", exact: true }),
  ).toBeDisabled();
  await page.getByRole("button", { name: "Próxima", exact: true }).click();
  await expect(
    page.getByRole("button", { name: "Próxima", exact: true }),
  ).toBeDisabled();
  await expect(
    page.getByRole("button", { name: "Anterior", exact: true }),
  ).toBeEnabled();
  await page.getByRole("combobox", { name: "Status", exact: true }).click();
  const response = page.waitForRequest((req) => {
    const url = new URL(req.url());
    return (
      url.searchParams.get("status") === "open" &&
      url.searchParams.get("page") === "1"
    );
  });
  await page.getByRole("option", { name: "Aberta", exact: true }).click();
  await response;
});

test("estados de carregamento, vazio e erro recuperável", async ({ page }) => {
  let release: () => void = () => {};
  const gate = new Promise<void>((resolve) => {
    release = resolve;
  });
  let mode = "empty";
  await page.route("**/api/support-requests?**", async (route) => {
    await gate;
    if (mode === "error")
      return route.fulfill({
        status: 503,
        json: { detail: "Serviço temporariamente indisponível." },
      });
    await route.fulfill({
      json: { ...paged, items: [], totalItems: 0, totalPages: 0 },
    });
  });
  await page.reload({ waitUntil: "domcontentloaded" });
  try {
    await expect(page.getByText("Carregando solicitações...")).toBeVisible();
  } finally {
    release();
  }
  await expect(page.getByText("Nenhuma solicitação cadastrada.")).toBeVisible();
  mode = "error";
  await page.reload();
  await expect(
    page.getByText("Serviço temporariamente indisponível."),
  ).toBeVisible();
  mode = "empty";
  await page.getByRole("button", { name: "Tentar novamente" }).click();
  await expect(page.getByText("Nenhuma solicitação cadastrada.")).toBeVisible();
});
