import { Modal } from "@/components/Modal";
import { SelectField } from "@/components/Select";
import { useSupportRequest } from "@/hooks/useSupportRequest";
import { useSupportRequestMutations } from "@/hooks/useSupportRequestMutations";
import { LoadingState } from "@/components/states/LoadingState";
import { ErrorState } from "@/components/states/ErrorState";
import { priorityLabels, statusLabels } from "@/utils/supportRequestLabels";
import { formatDate } from "@/utils/dateFormatter";
import type {
  RequestId,
  SupportRequestPriority,
  SupportRequestStatus,
} from "@/types/supportRequest.types";
export function SupportRequestDetailsModal({
  id,
  onClose,
}: {
  id: RequestId;
  onClose: () => void;
}) {
  const query = useSupportRequest(id);
  const mutations = useSupportRequestMutations();
  const request = query.data;
  const busy = mutations.priority.isPending || mutations.status.isPending;
  return (
    <Modal
      title="Detalhes da Solicitação"
      description={`Solicitação #${id}`}
      onClose={onClose}
      busy={busy}
    >
      {query.isPending ? (
        <LoadingState />
      ) : query.isError ? (
        <ErrorState error={query.error} retry={() => void query.refetch()} />
      ) : (
        request && (
          <div className="space-y-5">
            <div>
              <p className="label">Título</p>
              <h3 className="break-words text-lg font-semibold">
                {request.title}
              </h3>
            </div>
            <div>
              <p className="label">Solicitante</p>
              <p className="break-words">{request.requester}</p>
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              <div>
                <label className="label" htmlFor="detail-priority">
                  Alterar prioridade
                </label>
                <SelectField
                  id="detail-priority"
                  label="Alterar prioridade"
                  options={priorityLabels}
                  value={request.priority}
                  disabled={busy || query.isFetching}
                  onChange={(value) => {
                    if (value !== request.priority)
                      mutations.priority.mutate({
                        id,
                        value: value as SupportRequestPriority,
                      });
                  }}
                />
              </div>
              <div>
                <label className="label" htmlFor="detail-status">
                  Alterar status
                </label>
                <SelectField
                  id="detail-status"
                  label="Alterar status"
                  options={
                    request.status === "completed"
                      ? {
                          inProgress: statusLabels.inProgress,
                          completed: statusLabels.completed,
                        }
                      : statusLabels
                  }
                  value={request.status}
                  disabled={busy || query.isFetching}
                  onChange={(value) => {
                    if (value !== request.status)
                      mutations.status.mutate({
                        id,
                        value: value as SupportRequestStatus,
                      });
                  }}
                />
              </div>
            </div>
            <dl className="grid gap-4 rounded-xl bg-slate-50 p-4 text-sm sm:grid-cols-2">
              <div>
                <dt className="text-slate-500">Data de criação</dt>
                <dd className="mt-1">{formatDate(request.createdAt)}</dd>
              </div>
              <div>
                <dt className="text-slate-500">Data de conclusão</dt>
                <dd className="mt-1">{formatDate(request.completedAt)}</dd>
              </div>
            </dl>
            <div>
              <p className="label">Descrição</p>
              <p className="max-h-64 overflow-auto whitespace-pre-wrap break-words rounded-xl border border-slate-200 p-4 text-sm leading-7">
                {request.description}
              </p>
            </div>
          </div>
        )
      )}
    </Modal>
  );
}
