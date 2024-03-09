/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["../**/*{html,razor,js,cs}"],
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
                    '50': '#effafc',
                    '100': '#d6f0f7',
                    '200': '#b1e2f0',
                    '300': '#7ccce4',
                    '400': '#3facd1',
                    '500': '#2596be',
                    '600': '#20749a',
                    '700': '#215e7d',
                    '800': '#234f67',
                    '900': '#214358',
                    '950': '#112a3b'
                }
            }

        }
    },
    plugins: [],
}