import { beforeEach, it, expect, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "react-hot-toast";
import { SupportRequestsDashboard } from "@/components/SupportRequestsDashboard";
import * as service from "@/services/supportRequestService";
import { request, paged } from "../fixtures";
vi.mock("@/services/supportRequestService");
beforeEach(() => {
  vi.resetAllMocks();
  vi.mocked(service.getSupportRequests).mockResolvedValue(paged);
  vi.mocked(service.createSupportRequest).mockResolvedValue({
    ...request,
    id: 2,
    title: "Falha na rede",
  });
});
function setup() {
  const client = new QueryClient({
    defaultOptions: { queries: { retry: false }, mutations: { retry: false } },
  });
  render(
    <QueryClientProvider client={client}>
      <SupportRequestsDashboard />
      <Toaster />
    </QueryClientProvider>,
  );
  return userEvent.setup();
}
it("valida o formulário, cria uma vez e atualiza a listagem", async () => {
  const user = setup();
  expect(await screen.findAllByText(request.title)).toHaveLength(2);
  await user.click(screen.getByRole("button", { name: "Nova Solicitação" }));
  await user.click(screen.getByRole("button", { name: "Criar Solicitação" }));
  expect(await screen.findByText("Título é obrigatório.")).toBeVisible();
  expect(service.createSupportRequest).not.toHaveBeenCalled();
  await user.type(screen.getByLabelText(/^Título/), "Falha na rede");
  await user.type(screen.getByLabelText(/^Solicitante/), "João");
  await user.type(screen.getByLabelText(/^Descrição/), "Sem conexão na sala.");
  await user.click(screen.getByRole("button", { name: "Criar Solicitação" }));
  await waitFor(() =>
    expect(service.createSupportRequest).toHaveBeenCalledTimes(1),
  );
  expect(vi.mocked(service.createSupportRequest).mock.calls[0][0]).toEqual({
    title: "Falha na rede",
    requester: "João",
    description: "Sem conexão na sala.",
    priority: "medium",
  });
  await waitFor(() =>
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument(),
  );
  expect(service.getSupportRequests).toHaveBeenCalledTimes(2);
});
it("mostra erro e permite tentar novamente", async () => {
  vi.mocked(service.getSupportRequests).mockRejectedValueOnce(
    new Error("offline"),
  );
  const user = setup();
  expect(await screen.findByRole("alert")).toHaveTextContent(
    "Erro ao carregar solicitações.",
  );
  await user.click(screen.getByRole("button", { name: "Tentar novamente" }));
  expect(await screen.findAllByText(request.title)).toHaveLength(2);
});
it("usa busca com debounce e sem espaços nas bordas", async () => {
  const user = setup();
  await screen.findAllByText(request.title);
  await user.type(
    screen.getByRole("textbox", {
      name: "Pesquisar por título ou solicitante",
    }),
    " Maria ",
  );
  await waitFor(() =>
    expect(service.getSupportRequests).toHaveBeenLastCalledWith({
      page: 1,
      limit: 10,
      search: "Maria",
    }),
  );
});
