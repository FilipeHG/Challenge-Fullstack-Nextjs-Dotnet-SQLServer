import type {
  PriorityApiValue,
  StatusApiValue,
  SupportRequest,
  SupportRequestWire,
  SupportRequestPriority,
  SupportRequestStatus,
} from "@/types/supportRequest.types";
export function normalizePriority(
  value: PriorityApiValue,
): SupportRequestPriority {
  const mapped =
    ({ 1: "low", 2: "medium", 3: "high" } as const)[value as 1 | 2 | 3] ??
    value;
  if (mapped === "low" || mapped === "medium" || mapped === "high")
    return mapped;
  throw new Error("Prioridade inválida na resposta da API.");
}
export function normalizeStatus(value: StatusApiValue): SupportRequestStatus {
  const mapped =
    ({ 1: "open", 2: "inProgress", 3: "completed" } as const)[
      value as 1 | 2 | 3
    ] ?? value;
  if (mapped === "open" || mapped === "inProgress" || mapped === "completed")
    return mapped;
  throw new Error("Status inválido na resposta da API.");
}
export function mapSupportRequest(value: SupportRequestWire): SupportRequest {
  return {
    ...value,
    priority: normalizePriority(value.priority),
    status: normalizeStatus(value.status),
  };
}
