import axios from "axios";
import type { ProblemDetails } from "@/types/api.types";
export function formatError(error: unknown): string {
  if (axios.isAxiosError<ProblemDetails>(error)) {
    if (!error.response)
      return "Não foi possível conectar à API. Verifique se o backend está em execução.";
    const data = error.response.data;
    if (typeof data?.detail === "string" && data.detail) return data.detail;
    if (data?.errors && typeof data.errors === "object") {
      const messages = Object.values(data.errors)
        .flat()
        .filter((value): value is string => typeof value === "string");
      if (messages.length) return messages.join(" ");
    }
    if (typeof data?.title === "string" && data.title) return data.title;
  }
  return "Ocorreu um erro inesperado. Tente novamente.";
}
