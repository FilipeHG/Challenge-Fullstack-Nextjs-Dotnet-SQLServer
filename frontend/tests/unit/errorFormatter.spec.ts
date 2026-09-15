import { it, expect } from "vitest";
import { formatError } from "@/utils/errorFormatter";
it("prioriza detalhe e não expõe configuração de autenticação", () => {
  expect(
    formatError({
      isAxiosError: true,
      response: { data: { detail: "Transição proibida.", title: "Conflito" } },
      config: { headers: { Authorization: "secret" } },
    }),
  ).toBe("Transição proibida.");
});
it("normaliza validação, rede e falha desconhecida", () => {
  expect(
    formatError({
      isAxiosError: true,
      response: { data: { errors: { title: ["Título é obrigatório."] } } },
    }),
  ).toBe("Título é obrigatório.");
  expect(formatError({ isAxiosError: true })).toContain("conectar à API");
  expect(formatError(new Error("stack private"))).toBe(
    "Ocorreu um erro inesperado. Tente novamente.",
  );
});
