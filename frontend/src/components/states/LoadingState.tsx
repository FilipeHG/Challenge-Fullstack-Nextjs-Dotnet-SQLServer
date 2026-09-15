import { Loader2 } from "lucide-react";
export function LoadingState() {
  return (
    <div role="status" className="state">
      <Loader2 className="mx-auto mb-3 animate-spin text-indigo-500" />
      Carregando solicitações...
    </div>
  );
}
