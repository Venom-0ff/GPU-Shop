"use strict";(self["webpackChunkcasestudy"]=self["webpackChunkcasestudy"]||[]).push([[607],{5572:(e,t,a)=>{a.d(t,{C:()=>o,_:()=>n});const s="/api/",n=async e=>{let t,a=r();try{let n=await fetch(`${s}${e}`,{method:"GET",headers:a});t=await n.json()}catch(n){console.log(n),t={error:`Error has occured: ${n.message}`}}return t},o=async(e,t)=>{let a,n=r();try{let o=await fetch(`${s}${e}`,{method:"POST",headers:n,body:JSON.stringify(t)});a=await o.json()}catch(o){a=o}return a},r=()=>{const e=new Headers,t=JSON.parse(sessionStorage.getItem("customer"));return t?(e.append("Content-Type","application/json"),e.append("Authorization","Bearer "+t.token)):e.append("Content-Type","application/json"),e}},2607:(e,t,a)=>{a.r(t),a.d(t,{default:()=>g});var s=a(9835),n=a(1957);const o={class:"text-center q-mt-md"},r=(0,s._)("div",{class:"text-h4"},"Branches",-1),l=(0,s._)("br",null,null,-1),d={style:{height:"50vh",width:"90vw","margin-left":"5vw",border:"solid"},ref:"mapRef"};function c(e,t,a,c,i,p){const u=(0,s.up)("q-input"),m=(0,s.up)("q-btn");return(0,s.wg)(),(0,s.iD)("div",o,[r,(0,s._)("div",null,[(0,s.Wm)(u,{class:"q-ma-lg text-h5",placeholder:"enter current address",id:"address",modelValue:c.state.address,"onUpdate:modelValue":t[0]||(t[0]=e=>c.state.address=e)},null,8,["modelValue"]),l]),(0,s.Wm)(m,{label:"3 closest branches",onClick:c.genMap,class:"q-mb-md",style:{width:"30vw"}},null,8,["onClick"]),(0,s.wy)((0,s._)("div",d,null,512),[[n.F8,!0===c.state.showmap]])])}var i=a(5572),p=a(499);const u={name:"Map",setup(){const e=(0,p.iH)(null);let t=(0,p.qj)({status:"",address:"",showmap:!1});const a=async()=>{try{t.showmap=!0;const a=window.tt;let s=`https://api.tomtom.com/search/2/geocode/${t.address}.json?key=BBeWSjfQISvyCIvaYAAC3m8Xm9MG01w3`,n=await fetch(s),o=await n.json(),r=o.results[0].position.lat,l=o.results[0].position.lon;const d=await(0,i._)(`branch/${r}/${l}`);let c=a.map({key:"BBeWSjfQISvyCIvaYAAC3m8Xm9MG01w3",container:e.value,source:"vector/1/basic-main",center:[l,r],zoom:8});c.addControl(new a.FullscreenControl),c.addControl(new a.NavigationControl),d.forEach((e=>{let t=(new a.Marker).setLngLat([e.longitude,e.latitude]).addTo(c),s=25,n=new a.Popup({offset:s});n.setHTML(`<div id="popup">Branch#: ${e.id}</div><div>${e.street}, ${e.city}<br/>${e.distance.toFixed(2)} km</div>`),t.setPopup(n)}))}catch(a){t.status=a.message}};return{mapRef:e,state:t,genMap:a}}};var m=a(1639),h=a(6611),w=a(4455),v=a(9984),y=a.n(v);const f=(0,m.Z)(u,[["render",c]]),g=f;y()(u,"components",{QInput:h.Z,QBtn:w.Z})}}]);