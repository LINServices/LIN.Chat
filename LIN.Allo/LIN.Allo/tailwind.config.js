/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["D:/LIN Services/Components/LIN.Emma.UI/**/*{html,razor,js,cs}", "../**/*{html,razor,js,cs}", "D:/LIN Services/Components/LIN.Allo.Shared/**/*{html,razor,js,cs}"],
    theme: {
        screens: {
            'sm': '640px',
            'md': '768px',
            'dl': '910px',
            'lg': '1024px',
            'xl': '1280px',
            '2xl': '1536px',
        },
        extend: {
            colors: {

                light: "#F7F8FD",

                'current': {
                    '50': '#eef2ff',
                    '100': '#e0e7ff',
                    '200': '#c7d2fe',
                    '300': '#a5b4fc',
                    '400': '#818cf8',
                    '500': '#6366f1',
                    '600': '#4f46e5',
                    '700': '#4338ca',
                    '800': '#3730a3',
                    '900': '#312e81',
                    '950': '#1e1b4b'
                }
            }

        }
    },
    plugins: [],
}