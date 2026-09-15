export const SupportRequestPriority = {
  Low: "low",
  Medium: "medium",
  High: "high",
} as const;
export type SupportRequestPriority =
  (typeof SupportRequestPriority)[keyof typeof SupportRequestPriority];
export const SupportRequestStatus = {
  Open: "open",
  InProgress: "inProgress",
  Completed: "completed",
} as const;
export type SupportRequestStatus =
  (typeof SupportRequestStatus)[keyof typeof SupportRequestStatus];
export type RequestId = number | string;
export interface SupportRequest {
  id: RequestId;
  title: string;
  description: string;
  requester: string;
  priority: SupportRequestPriority;
  status: SupportRequestStatus;
  createdAt: string;
  completedAt: string | null;
}
export type PriorityApiValue = 1 | 2 | 3 | SupportRequestPriority;
export type StatusApiValue = 1 | 2 | 3 | SupportRequestStatus;
export interface SupportRequestWire extends Omit<
  SupportRequest,
  "priority" | "status"
> {
  priority: PriorityApiValue;
  status: StatusApiValue;
}
export interface CreateSupportRequestPayload {
  title: string;
  description: string;
  requester: string;
  priority: SupportRequestPriority;
}
export interface SupportRequestListQuery {
  page: number;
  limit: number;
  status?: SupportRequestStatus;
  priority?: SupportRequestPriority;
  search?: string;
}
export interface UpdateSupportRequestPriorityPayload {
  priority: SupportRequestPriority;
}
export interface UpdateSupportRequestStatusPayload {
  status: SupportRequestStatus;
}
