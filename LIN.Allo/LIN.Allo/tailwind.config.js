/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["D:/LIN/LIN Services/Components/LIN.Emma.UI/**/*{html,razor,js,ts,cs}", "../**/*{html,razor,js,ts,cs}", "D:/LIN/LIN Services/Components/LIN.Allo.Shared/**/*{html,razor,js,ts,cs}"],
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
                primaryDark: "#121B22",

                'current': {
                    '50': '#f2f5fc',
                    '100': '#e2e9f7',
                    '200': '#cbd8f2',
                    '300': '#a7bfe9',
                    '400': '#7d9edd',
                    '500': '#5e7ed3',
                    '600': '#4a64c6',
                    '700': '#4053b5',
                    '800': '#394694',
                    '900': '#303a70',
                    '950': '#232748'
                }
            },
            keyframes: {
                progress: { "0%": { transform: "translateX(-100%)" }, "100%": { transform: "translateX(100%)" } }
            },
            animation: {
                progress: "progress 2s linear infinite"
            }
        }
    },
    plugins: [],
}