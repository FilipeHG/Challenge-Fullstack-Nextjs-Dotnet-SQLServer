import { useQuery } from "@tanstack/react-query";
import { getApiHealth } from "@/services/healthService";
export function useApiHealth() {
  return useQuery({
    queryKey: ["health"],
    queryFn: getApiHealth,
    retry: 1,
    refetchInterval: 30000,
    enabled: typeof window !== "undefined",
  });
}
