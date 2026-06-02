import Navbar from "@/components/layout/Navbar";
import Sidebar from "@/components/layout/Sidebar";
import GlobalToaster from "@/components/providers/GlobalToaster";
import AuthProvider from "@/providers/AuthProvider";
import QueryProvider from "@/providers/QueryProvider";
import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
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
  title: "Sistema Contable — L.A.M.A. Medellín",
  description: "Plataforma integral de gestión financiera del Capítulo Región Norte",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        <AuthProvider>
          <QueryProvider>
            {/* App Shell: Sidebar fijo + área principal con Navbar */}
            <div className="flex h-screen overflow-hidden bg-slate-50">
              <Sidebar />
              <div className="flex flex-1 flex-col overflow-hidden">
                <Navbar />
                {/* Área de contenido con scroll independiente */}
                <main className="flex-1 overflow-y-auto">
                  {children}
                </main>
              </div>
            </div>
            <GlobalToaster />
          </QueryProvider>
        </AuthProvider>
      </body>
    </html>
  );
}
