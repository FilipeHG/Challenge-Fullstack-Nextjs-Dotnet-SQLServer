import { describe, it, expect } from "vitest";
import { supportRequestSchema } from "@/schemas/supportRequest.schema";
const valid = {
  title: " Rede ",
  description: " Sem conexão ",
  requester: " Maria ",
  priority: "high",
};
describe("validação de criação", () => {
  it("normaliza espaços e aceita dados válidos", () => {
    expect(supportRequestSchema.parse(valid)).toEqual({
      ...valid,
      title: "Rede",
      description: "Sem conexão",
      requester: "Maria",
    });
  });
  it.each(["title", "description", "requester"])(
    "rejeita %s vazio",
    (field) => {
      expect(
        supportRequestSchema.safeParse({ ...valid, [field]: "   " }).success,
      ).toBe(false);
    },
  );
  it.each([
    ["title", 200],
    ["requester", 200],
    ["description", 4000],
  ] as const)("limita %s", (field, limit) => {
    expect(
      supportRequestSchema.safeParse({ ...valid, [field]: "a".repeat(limit) })
        .success,
    ).toBe(true);
    expect(
      supportRequestSchema.safeParse({
        ...valid,
        [field]: "a".repeat(limit + 1),
      }).success,
    ).toBe(false);
  });
  it("rejeita prioridade desconhecida", () => {
    expect(
      supportRequestSchema.safeParse({ ...valid, priority: "urgent" }).success,
    ).toBe(false);
  });
});
