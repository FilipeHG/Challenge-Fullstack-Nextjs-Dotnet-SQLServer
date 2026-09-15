import { apiClient } from "./apiClient";
import { mapSupportRequest } from "@/utils/supportRequestMapper";
import type {
  CreateSupportRequestPayload,
  RequestId,
  SupportRequestListQuery,
  SupportRequestWire,
  UpdateSupportRequestPriorityPayload,
  UpdateSupportRequestStatusPayload,
} from "@/types/supportRequest.types";
import type { PagedResult } from "@/types/api.types";
const path = (id: RequestId) => `/support-requests/${encodeURIComponent(id)}`;
export async function getSupportRequests(query: SupportRequestListQuery) {
  const { data } = await apiClient.get<PagedResult<SupportRequestWire>>(
    "/support-requests",
    { params: { ...query, search: query.search?.trim() || undefined } },
  );
  return { ...data, items: data.items.map(mapSupportRequest) };
}
export async function getSupportRequestById(id: RequestId) {
  return mapSupportRequest(
    (await apiClient.get<SupportRequestWire>(path(id))).data,
  );
}
export async function createSupportRequest(
  payload: CreateSupportRequestPayload,
) {
  return mapSupportRequest(
    (await apiClient.post<SupportRequestWire>("/support-requests", payload))
      .data,
  );
}
export async function updateSupportRequestPriority(
  id: RequestId,
  payload: UpdateSupportRequestPriorityPayload,
) {
  return mapSupportRequest(
    (await apiClient.patch<SupportRequestWire>(`${path(id)}/priority`, payload))
      .data,
  );
}
export async function updateSupportRequestStatus(
  id: RequestId,
  payload: UpdateSupportRequestStatusPayload,
) {
  return mapSupportRequest(
    (await apiClient.patch<SupportRequestWire>(`${path(id)}/status`, payload))
      .data,
  );
}
export async function deleteSupportRequest(id: RequestId) {
  await apiClient.delete(path(id));
}
