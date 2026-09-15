import * as AlertDialog from "@radix-ui/react-alert-dialog";
import { Trash2 } from "lucide-react";
import { useSupportRequestMutations } from "@/hooks/useSupportRequestMutations";
import type { SupportRequest } from "@/types/supportRequest.types";
export function DeleteSupportRequestDialog({
  request,
  onClose,
  onDeleted,
}: {
  request: SupportRequest;
  onClose: () => void;
  onDeleted: () => void;
}) {
  const { remove } = useSupportRequestMutations();
  return (
    <AlertDialog.Root
      open
      onOpenChange={(open) => {
        if (!open && !remove.isPending) onClose();
      }}
    >
      <AlertDialog.Portal>
        <AlertDialog.Overlay className="overlay" />
        <AlertDialog.Content className="modal">
          <div className="mb-4 w-fit rounded-full bg-rose-50 p-3 text-rose-700">
            <Trash2 />
          </div>
          <AlertDialog.Title className="text-xl font-semibold">
            Excluir solicitação?
          </AlertDialog.Title>
          <AlertDialog.Description className="my-4 text-sm leading-6 text-slate-600">
            Esta ação é permanente. Somente solicitações abertas podem ser
            excluídas.
          </AlertDialog.Description>
          <p className="mb-6 break-words rounded-xl bg-slate-50 p-4 text-sm">
            {request.title} · {request.requester}
          </p>
          <div className="flex justify-end gap-3">
            <AlertDialog.Cancel
              className="btn-secondary"
              disabled={remove.isPending}
            >
              Cancelar
            </AlertDialog.Cancel>
            <button
              className="btn-danger"
              disabled={remove.isPending}
              onClick={() => {
                if (!remove.isPending)
                  remove.mutate(request.id, { onSuccess: onDeleted });
              }}
            >
              {remove.isPending ? "Excluindo..." : "Excluir solicitação"}
            </button>
          </div>
        </AlertDialog.Content>
      </AlertDialog.Portal>
    </AlertDialog.Root>
  );
}
