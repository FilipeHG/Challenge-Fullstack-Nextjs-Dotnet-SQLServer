import { Eye, Trash2 } from "lucide-react";
import type { SupportRequest } from "@/types/supportRequest.types";
export interface RequestActionsProps {
  request: SupportRequest;
  onDetails: (request: SupportRequest) => void;
  onDelete: (request: SupportRequest) => void;
}
export function RequestActions({
  request,
  onDetails,
  onDelete,
}: RequestActionsProps) {
  return (
    <div className="flex items-center gap-1">
      <button
        className="icon-button"
        aria-label={`Ver detalhes de ${request.title}`}
        title="Ver detalhes"
        onClick={() => onDetails(request)}
      >
        <Eye size={17} />
      </button>
      <button
        className="icon-button text-rose-700"
        disabled={request.status !== "open"}
        aria-label={`Excluir ${request.title}`}
        title={
          request.status === "open"
            ? "Excluir solicitação"
            : "Somente solicitações abertas podem ser excluídas."
        }
        onClick={() => onDelete(request)}
      >
        <Trash2 size={16} />
      </button>
    </div>
  );
}
