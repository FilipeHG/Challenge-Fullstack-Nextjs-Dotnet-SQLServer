import type { ReactNode } from "react";
import { Headphones, Activity } from "lucide-react";
import { useApiHealth } from "@/hooks/useApiHealth";
export function DashboardLayout({ children }: { children: ReactNode }) {
  const health = useApiHealth();
  const state = health.isPending
    ? "checking"
    : health.isError
      ? "offline"
      : "online";
  return (
    <div className="min-h-screen">
      <a href="#main" className="sr-only focus:not-sr-only">
        Ir para o conteúdo
      </a>
      <header className="border-b border-slate-200 bg-white/90">
        <div className="mx-auto flex max-w-7xl items-center justify-between gap-3 px-5 py-5 sm:px-8">
          <div className="flex items-center gap-3">
            <div className="rounded-xl bg-indigo-600 p-2.5 text-white">
              <Headphones size={24} />
            </div>
            <div>
              <p className="font-bold tracking-tight text-slate-900">
                Support Requests
              </p>
              <p className="hidden text-xs text-slate-500 sm:block">
                Dashboard de solicitações internas
              </p>
            </div>
          </div>
          <div className="text-right text-xs">
            <p className="mb-1 text-slate-500">API Status</p>
            <p role="status" className="flex items-center gap-1.5 font-medium">
              {state === "checking" ? (
                <Activity size={12} className="animate-pulse" />
              ) : (
                <span
                  className={`h-2 w-2 rounded-full ${state === "online" ? "bg-emerald-500" : "bg-rose-500"}`}
                />
              )}{" "}
              {state === "checking"
                ? "Verificando"
                : state === "online"
                  ? "Online"
                  : "Offline"}
            </p>
          </div>
        </div>
      </header>
      <main id="main" className="mx-auto max-w-7xl px-5 py-10 sm:px-8">
        {children}
      </main>
      <footer className="mx-auto flex max-w-7xl flex-wrap justify-between gap-2 px-5 py-8 text-xs text-slate-500 sm:px-8">
        <span>© {new Date().getFullYear()} Support Requests</span>
        <span>v1.0.0 · Next.js, React, TypeScript, Tailwind</span>
      </footer>
    </div>
  );
}
