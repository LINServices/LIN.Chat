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
                'shark': {
                    '50': '#f5f6f6',
                    '100': '#e4e6e9',
                    '200': '#ccd1d5',
                    '300': '#a9b1b7',
                    '400': '#7e8992',
                    '500': '#636e77',
                    '600': '#555d65',
                    '700': '#494e55',
                    '800': '#40444a',
                    '900': '#393c40',
                    '950': '#1f2124'
                }
            }
        }
    }
}