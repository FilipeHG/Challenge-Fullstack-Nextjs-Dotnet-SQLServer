import { Inbox } from "lucide-react";
export function EmptyState({ filtered }: { filtered: boolean }) {
  return (
    <div className="state">
      <Inbox size={36} className="mx-auto mb-4 text-slate-400" />
      <p className="font-medium text-slate-700">
        {filtered
          ? "Nenhuma solicitação encontrada para os filtros informados."
          : "Nenhuma solicitação cadastrada."}
      </p>
      {!filtered && (
        <p className="mt-2 text-sm">
          Crie a primeira solicitação para começar.
        </p>
      )}
    </div>
  );
}
