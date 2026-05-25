// // Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// // for details on configuring this project to bundle and minify static web assets.

// // Write your JavaScript code.
// async function loadNews(query = null, search = null) {

//     const response =
//         await fetch(`Home/News?q=${query}`);

//     const news = await response.json();

//     console.log(news);

//     const container =
//         document.getElementById("news");
//     container.innerHTML = "";

//     news.forEach(item => {

//         container.innerHTML += `
//             <div>
//                 <h3>${item.title}</h3>

//                 <a href="${item.link}" target="_blank">
//                     Read More
//                 </a>

//                 <hr>
//             </div>
//         `;
//     });
// }

// loadNews("Iron Steel");

