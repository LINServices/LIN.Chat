/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["../**/*{html,razor,js,cs}"],
    theme: {
        extend: {
            width: {
                'one-third': '33.333333%',
                'ring-1.5':"box-shadow: var(--tw-ring-inset) 0 0 0 calc(1.5px + var(--tw-ring-offset-width)) var(--tw-ring-color);"
            }
        },
    },
    plugins: [],
}

