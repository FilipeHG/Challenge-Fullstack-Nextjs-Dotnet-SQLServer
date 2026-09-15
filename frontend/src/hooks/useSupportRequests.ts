import { useQuery } from "@tanstack/react-query";
import { getSupportRequests } from "@/services/supportRequestService";
import type { SupportRequestListQuery } from "@/types/supportRequest.types";
export function useSupportRequests(query: SupportRequestListQuery) {
  return useQuery({
    queryKey: ["supportRequests", query],
    queryFn: () => getSupportRequests(query),
    enabled: typeof window !== "undefined",
  });
}
