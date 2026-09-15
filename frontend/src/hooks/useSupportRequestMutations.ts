import { useMutation, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";
import * as service from "@/services/supportRequestService";
import { formatError } from "@/utils/errorFormatter";
import type {
  RequestId,
  SupportRequestPriority,
  SupportRequestStatus,
} from "@/types/supportRequest.types";
export function useSupportRequestMutations() {
  const client = useQueryClient();
  async function success(message: string, id?: RequestId) {
    toast.success(message);
    await Promise.all([
      client.invalidateQueries({ queryKey: ["supportRequests"] }),
      ...(id === undefined
        ? []
        : [client.invalidateQueries({ queryKey: ["supportRequest", id] })]),
    ]);
  }
  const onError = (error: unknown) => {
    toast.error(formatError(error));
  };
  const create = useMutation({
    mutationFn: service.createSupportRequest,
    onSuccess: () => success("Solicitação criada com sucesso."),
    onError,
  });
  const priority = useMutation({
    mutationFn: ({
      id,
      value,
    }: {
      id: RequestId;
      value: SupportRequestPriority;
    }) => service.updateSupportRequestPriority(id, { priority: value }),
    onSuccess: (_, v) => success("Prioridade atualizada com sucesso.", v.id),
    onError,
  });
  const status = useMutation({
    mutationFn: ({
      id,
      value,
    }: {
      id: RequestId;
      value: SupportRequestStatus;
    }) => service.updateSupportRequestStatus(id, { status: value }),
    onSuccess: (_, v) => success("Status atualizado com sucesso.", v.id),
    onError,
  });
  const remove = useMutation({
    mutationFn: service.deleteSupportRequest,
    onSuccess: (_, id) => {
      client.removeQueries({ queryKey: ["supportRequest", id] });
      return success("Solicitação excluída com sucesso.");
    },
    onError,
  });
  return { create, priority, status, remove };
}
