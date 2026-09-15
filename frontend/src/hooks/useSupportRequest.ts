import { useQuery } from "@tanstack/react-query";
import { getSupportRequestById } from "@/services/supportRequestService";
import type { RequestId } from "@/types/supportRequest.types";
export function useSupportRequest(id: RequestId) {
  return useQuery({
    queryKey: ["supportRequest", id],
    queryFn: () => getSupportRequestById(id),
    refetchOnMount: "always",
    enabled: typeof window !== "undefined",
  });
}
