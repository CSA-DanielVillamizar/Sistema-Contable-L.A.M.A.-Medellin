'use client';

import { Toaster } from 'react-hot-toast';

export default function GlobalToaster() {
    return (
        <Toaster
            position="top-right"
            toastOptions={{
                duration: 3500,
                style: {
                    borderRadius: '0.75rem',
                    border: '1px solid #dbeafe',
                    background: '#ffffff',
                    color: '#0f172a',
                    boxShadow: '0 10px 30px rgba(2, 6, 23, 0.12)',
                },
                success: {
                    iconTheme: {
                        primary: '#15803d',
                        secondary: '#ffffff',
                    },
                },
            }}
        />
    );
}
