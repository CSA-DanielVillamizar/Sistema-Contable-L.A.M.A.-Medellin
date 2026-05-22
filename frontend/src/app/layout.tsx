import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import QueryProvider from "@/providers/QueryProvider";
import AuthProvider from "@/providers/AuthProvider";
import Sidebar from "@/components/layout/Sidebar";
import Navbar from "@/components/layout/Navbar";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Sistema Contable – L.A.M.A. Medellín",
  description: "Plataforma contable y de gestión para la Fundación L.A.M.A. Medellín",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="es" suppressHydrationWarning>
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        <AuthProvider>
          <QueryProvider>
            {/* ── App Shell ──────────────────────────────────────────────── */}
            <div className="flex h-screen overflow-hidden bg-slate-50">
              {/* Sidebar fijo a la izquierda */}
              <Sidebar />

              {/* Área principal: Navbar + contenido con scroll independiente */}
              <div className="flex flex-1 flex-col overflow-hidden">
                <Navbar />
                <main className="flex-1 overflow-y-auto">
                  {children}
                </main>
              </div>
            </div>
          </QueryProvider>
        </AuthProvider>
      </body>
    </html>
  );
}

