import { Badge } from "@/components/Badge";
import {
  RequestActions,
  type RequestActionsProps,
} from "@/components/RequestActions";
import { formatDate } from "@/utils/dateFormatter";
export function SupportRequestMobileCard(props: RequestActionsProps) {
  const { request } = props;
  return (
    <article className="border-t border-slate-100 p-5 md:hidden">
      <h3 className="break-words font-semibold text-slate-900">
        {request.title}
      </h3>
      <p className="mt-1 break-words text-sm text-slate-500">
        {request.requester}
      </p>
      <div className="my-4 flex flex-wrap gap-2">
        <Badge value={request.priority} />
        <Badge value={request.status} />
      </div>
      <div className="flex items-end justify-between gap-2">
        <div className="text-xs leading-6 text-slate-500">
          <p>Criada em {formatDate(request.createdAt)}</p>
          {request.completedAt && (
            <p>Concluída em {formatDate(request.completedAt)}</p>
          )}
        </div>
        <RequestActions {...props} />
      </div>
    </article>
  );
}
