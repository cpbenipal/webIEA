'use strict';

var $ = require('jquery');

var _ = require('lodash');


var util = require('./util');


function Menu(eventBus, overlays, bpmnjs) {
  function getType(element){
    if(element.parent&&element.parent.businessObject){
    var elems= element.parent.businessObject.flowElements;
     var select=elems.find(e=>e.id=='Task_select');
     var type=select.$attrs["bpmn:sField"]||select.$attrs["sField"];
     if(type=='Persons'){
       type='Person';
     }else if(type=='Companies'){
       type='Company';
     }
     
     if(element.id!='Task_select'){
      var link=getLink(element,elems.filter(e=>e.$type=='bpmn:ManualTask'),element.id);
      if(link){
          type=link.$attrs["bpmn:sField"]||link.$attrs["sField"];
      }
    }
     return type;
  }
  return null;
   }
   function getLink(element,elems,id){
    var link=elems.find(el=>{
      if(el.outgoing&&(el.$type=='bpmn:ManualTask'||el.$type=='bpmn:ServiceTask')){
        for(var i=0;i<el.outgoing.length;i++){
          var outgoing=el.outgoing[i].targetRef;
          if(el.$type=='bpmn:ManualTask'&&outgoing.$type=="bpmn:UserTask"&&id==outgoing.id){
            return el;
          }else if(id==outgoing.id){
            return true;
          }else if(el.$type=='bpmn:ManualTask'&&outgoing.$type=="bpmn:ServiceTask"){
            if(getLink(outgoing,element.parent.businessObject.flowElements.filter(e=>e.id!=el.id),id)){
             return el;
            }
          }else if(el.$type=='bpmn:ServiceTask'&&outgoing.$type=="bpmn:ServiceTask"){
            return getLink(outgoing,element.parent.businessObject.flowElements.filter(e=>e.id!=el.id),id);
          }
        }
      }
      return false;
    });
    return link;
   }
  
  var setValues=function(e,element,$overlay){
    var $form=$(e.target).closest("form");
    var values=[];
    var settings={};
    settings.sField=$form.find(".settings-field").val();
    settings.sType=$form.find(".settings-type").val();
    settings.sValue=$form.find(".settings-value").val();
    settings.sDescription=$form.find(".settings-description").val();
    //select
    if($form.find(".edit-select").length>0){
      values.push("What do you want to get?");
    }else 
    //operatin
    if($form.find(".edit-operation").length>0){
    } else 
    //condition
    if($form.find(".edit-condition").length>0){
      var typeSelect= getType(element);
      values.push("Filter on a ");
      values.push(typeSelect);
      values.push(" field");
      settings.sType=typeSelect;
    }
    else 
    //link
    if($form.find(".edit-link").length>0){
      values.push("Consider linked data");
    }
      
    util.setValues(element, values,settings);
      
   
  }
  
  function createMenu(element) {
    var html=Menu.OVERLAY_HTMLDEFAULT;
    var type=getType(element);
    if(element.type=='bpmn:Task'&&element.id=="Task_select"){
      html=Menu.OVERLAY_HTMLSELECT;
    }else if(element.type=='bpmn:ServiceTask'){
      html=Menu.OVERLAY_HTMLOPERATION;
    }else if(element.type=='bpmn:UserTask'){
      var options="";
      
      if(type&&type.search("Person")>=0){
        options=$("#fp_personContactFields").html();
      }else  if(type&&type.search("Folder")>=0){
        options=$("#fp_folderContactFields").html();
      }else if(type){
        options=$("#fp_companyContactFields").html();
      }
    
      html=Menu.OVERLAY_HTMLCONDITION.replace("{options}",options);
    }else if(element.type=='bpmn:ManualTask'){
      var options="";
     
      
      if(type&&type.search("Person")>=0){
        options=$("#fp_personToPersonLinkTypes").html();
      }else if(type){
        options=$("#fp_companyToPersonLinkTypes").html();
      }
    
      html=Menu.OVERLAY_HTMLLINK.replace("{options}",options?options:"");
    }
      var $overlay = $(html);
     
      var  $buttonsOK = $overlay.find('.settings-button-ok');
      var  $buttonsCancel = $overlay.find('.settings-button-cancel');
      var $linkSettingsField=$overlay.find(".edit-link .settings-field");
      var $settingsInput=$overlay.find(".settings-input input");
      var $settingsSelect=$overlay.find(".settings-input select");
     
      
    
    $settingsInput.on('keyup',function(e){
        e.preventDefault();
        setValues(e,element,$overlay);
        eventBus.fire('fields.update');
    });
    $buttonsOK.on('click', function(e) {
      e.preventDefault();
      setValues(e,element,$overlay);
  });
  $settingsSelect.on('change',function(e){
    e.preventDefault();
        setValues(e,element,$overlay);
        if(element.type=='bpmn:Task'&&element.id=="Task_select"||element.type=='bpmn:ManualTask'&&$(e.target).hasClass('settings-field')){
          eventBus.fire('menu.update');
        }else{
          eventBus.fire('fields.update');
        }
        
  });
    function setType(sType,sField){
      var options="";
      var type=getType(element);
      var id="#fp_companyTo"+sField+"LinkTypes";
      if(type&&type.search("Person")>=0){
        id="#fp_personTo"+sField+"LinkTypes";
      }
      options=$(id).html().trim();
     var html=Menu.OVERLAY_HTMLLINKSettingsTypeOptions.replace("{options}",(options?options:""));
      sType.html(html);
    }
   

    // attach an overlay to a node
    overlays.add(element, 'comments', {
      position: {
        top: 50,
        left: 10
      },
      html: $overlay
    });

    
    var sField=element.businessObject.$attrs["bpmn:sField"]||element.businessObject.$attrs["sField"];
      var sValue=element.businessObject.$attrs["bpmn:sValue"]||element.businessObject.$attrs["sValue"];
      var sType=element.businessObject.$attrs["bpmn:sType"]||element.businessObject.$attrs["sType"];
      var sDescription=element.businessObject.$attrs["bpmn:sDescription"]||element.businessObject.$attrs["sDescription"];
      if(sField&&element.type=='bpmn:Task'&&element.id=="Task_select"
      ||sField&&sField.search(type)>=0
      ||sField&&element.type=='bpmn:ManualTask'
      ||sField&&element.type=='bpmn:ServiceTask'){
       $overlay.find(".settings-field").val(sField);
      }
      if(sValue){
       $overlay.find(".settings-value").val(sValue);
       }else{
         $overlay.find(".settings-value").val('');
       }
       if($($overlay).find(".settings-type").length>0&&sField&&element.type=='bpmn:ManualTask'){
        setType($($overlay).find(".settings-type"),sField);
       }
        if(sType&&$overlay.find(".settings-type [value='"+sType+"']").length>0){
          $overlay.find(".settings-type").val(sType);
        }else{
          $overlay.find(".settings-type").val('any');
        }
      
       if(sDescription){
        $overlay.find(".settings-description").val(sDescription);
        }else{
        $overlay.find(".settings-description").val('');
        }
  }
  //end createMenu
  eventBus.on('shape.added', function(event) {
    var element = event.element;

    if (element.labelTarget ||
       !element.businessObject.$instanceOf('bpmn:Task')) {

      return;
    }

    _.defer(function() {
      createMenu(element);
    });

  });

}
Menu.OVERLAY_HTMLDEFAULT =
  `<div class="menu-overlay hidden">
    
  </div>`;
  Menu.OVERLAY_HTMLCONDITION =
  `<div class="menu-overlay menu-overlay_condition">
    <form class="content">
      <div class="edit edit-condition">
      <div class="settings-row"> 
      <span class='settings-lable'>
      Field: 
      </span>
      <div class='settings-input'>
      <select class='settings-field '>
        {options}
      </select>
      </div>
      </div>
     
      <div class="settings-row "> 
      <span class='settings-lable'>
      Value: 
      </span>
      
      <div class="settings-input">
      <input class='settings-value' />
      </div>
      </div>
      <div class="settings-row hidden"> 
      <div class="settings-input">
      <input class='settings-type' />
      </div>
      </div>
      <div style="clear: left"></div>
      </div> 
     
    </form>
  </div>`;

  Menu.OVERLAY_HTMLSELECT =
  `<div class="menu-overlay menu-overlay_select">
    
    <form class="content">
      <div class="edit edit-select">
      <div class="settings-row"> 
      <span class='settings-lable'>
      Field: 
      </span>
      <div class='settings-input'>
      <select class='settings-field' >
        <option value="Persons">Persons</option>
        <option value="Companies">Companies</option>
      </select>
      </div>
      </div>
      <div style="clear: left"></div>
      </div> 
    </form>
  </div>`;
  Menu.OVERLAY_HTMLOPERATION =
  `<div class="menu-overlay menu-overlay_operation">
   
    <form class="content">
      <div class="edit edit-operation">
        <div class="settings-row"> 
         
          <div class='settings-input'>
            <select class='settings-field settings-input'>
              <option value="OR">OR</option>
              <option value="AND">AND</option>
              <option value="NOT">NOT</option>
            </select>
          </div>
        </div>
        
        <div style="clear: left"></div>
      </div> 
     
    </form>
  </div>`;
  Menu.OVERLAY_HTMLLINKSettingsTypeOptions=`
  <option value='any'>(any)</option>
    {options}
  `;
  Menu.OVERLAY_HTMLLINK =
  `<div class="menu-overlay menu-overlay_link">
    
    <form class="content">
      <div class="edit edit-link">
      <div class="settings-row"> 
          <span class='settings-lable'>
          To: 
          </span>
          <div class='settings-input'>
            <select class='settings-field'>
              <option value="Person">Person</option>
              <option value="Company">Company</option>
              <option value="Folder">Folder</option>
            </select>
          </div>
        </div>
        <div class="settings-row"> 
          <span class='settings-lable'>
          Type: 
          </span>
          <div class='settings-input'>
            <select class='settings-type'>
            `+Menu.OVERLAY_HTMLLINKSettingsTypeOptions+`
            </select>
          </div>
        </div>
        
        <div class="settings-row"> 
          <span class='settings-lable'>
          Description:
          </span>
          <div class='settings-input'>
          <input class='settings-description' />
          </div>
        </div>
        
        <div style="clear: left"></div>
      </div> 
     
    </form>
  </div>`;
// Menu.COMMENT_HTML =
//   '<div class="comment">' +
//     '<div data-text /><a href class="delete icon-delete" data-delete></a>' +
//   '</div>';

module.exports = Menu;
