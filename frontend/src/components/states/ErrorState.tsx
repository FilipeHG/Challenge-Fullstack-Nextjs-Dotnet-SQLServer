import { AlertCircle } from "lucide-react";
import { formatError } from "@/utils/errorFormatter";
export function ErrorState({
  error,
  retry,
}: {
  error: unknown;
  retry: () => void;
}) {
  return (
    <div role="alert" className="state">
      <AlertCircle className="mx-auto mb-3 text-rose-600" />
      <p className="font-semibold text-slate-800">
        Erro ao carregar solicitações.
      </p>
      <p className="my-3 text-sm">{formatError(error)}</p>
      <button className="btn-secondary" onClick={retry}>
        Tentar novamente
      </button>
    </div>
  );
}
