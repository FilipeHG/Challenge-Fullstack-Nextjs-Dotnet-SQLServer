import { useState } from "react";
import type { AppProps } from "next/app";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { Toaster } from "react-hot-toast";
import { DashboardLayout } from "@/layouts/DashboardLayout";
import "@/styles/globals.css";
export default function App({ Component, pageProps }: AppProps) {
  const [client] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: { retry: 1, refetchOnWindowFocus: false, staleTime: 30000 },
        },
      }),
  );
  return (
    <QueryClientProvider client={client}>
      <DashboardLayout>
        <Component {...pageProps} />
      </DashboardLayout>
      <Toaster position="top-right" />
      {process.env.NODE_ENV === "development" &&
        process.env.NEXT_PUBLIC_QUERY_DEVTOOLS === "true" && (
          <ReactQueryDevtools initialIsOpen={false} />
        )}
    </QueryClientProvider>
  );
}
