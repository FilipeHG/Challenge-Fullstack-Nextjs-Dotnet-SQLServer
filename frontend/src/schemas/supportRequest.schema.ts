import { z } from "zod";
export const supportRequestSchema = z.object({
  title: z
    .string()
    .trim()
    .min(1, "Título é obrigatório.")
    .max(200, "Título deve ter no máximo 200 caracteres."),
  requester: z
    .string()
    .trim()
    .min(1, "Solicitante é obrigatório.")
    .max(200, "Solicitante deve ter no máximo 200 caracteres."),
  description: z
    .string()
    .trim()
    .min(1, "Descrição é obrigatória.")
    .max(4000, "Descrição deve ter no máximo 4000 caracteres."),
  priority: z.enum(["low", "medium", "high"], {
    error: "Selecione uma prioridade válida.",
  }),
});
