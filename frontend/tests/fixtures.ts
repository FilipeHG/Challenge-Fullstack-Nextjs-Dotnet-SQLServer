import type { SupportRequest } from "@/types/supportRequest.types";
export const request: SupportRequest = {
  id: 1,
  title: "Impressora indisponível",
  description: "A impressora não está imprimindo.",
  requester: "Maria Silva",
  priority: "high",
  status: "open",
  createdAt: "2026-09-14T12:00:00Z",
  completedAt: null,
};
export const paged = {
  items: [request],
  page: 1,
  limit: 10,
  totalItems: 1,
  totalPages: 1,
  hasNextPage: false,
  hasPreviousPage: false,
};
