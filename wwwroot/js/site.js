// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code
 const castleImg = document.querySelector('.castleImg')
 const formPrincipal = document.querySelector('.formPrincipal')

 gsap.from(castleImg, {opacity: 0, scale: 0.5, duration: 1.5})
 gsap.from(formPrincipal, {opacity: 0, duration: 2})
 