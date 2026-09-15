import Head from "next/head";
import { SupportRequestsDashboard } from "@/components/SupportRequestsDashboard";
export default function Home() {
  return (
    <>
      <Head>
        <title>Support Requests · Solicitações de Suporte</title>
        <meta
          name="description"
          content="Dashboard para gerenciamento de solicitações internas de suporte."
        />
        <link rel="icon" href="/favicon.svg" />
      </Head>
      <SupportRequestsDashboard />
    </>
  );
}
