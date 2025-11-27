import { useEffect } from "react";
import { Outlet } from "react-router";
import Header from "~/components/Header";
import Footer from "~/components/Footer";

export default function Layout() {
    useEffect(() => {
        const html = document.documentElement;
        html.style.colorScheme = 'light';
        html.classList.remove('dark');
        html.setAttribute('data-theme', 'light');
        try {
            localStorage.setItem('theme', 'light');
        } catch (e) {}
    }, []);

    return (
        <div className="min-h-screen bg-white">
            <Header />
            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 mt-16">
                <Outlet />
            </main>
            <Footer />
        </div>
    );
}