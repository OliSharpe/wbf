<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:srwrt="http://schemas.microsoft.com/WebParts/v3/searchresults/runtime"
    xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime">
  <xsl:output method="xml" indent="no"/>
  <xsl:param name="Keyword" />
  <xsl:param name="ResultsBy" />
  <xsl:param name="ViewByUrl" />
  <xsl:param name="ShowDropDown" />
  <xsl:param name="ViewByValue" />
  <xsl:param name="SortBy" />
  <xsl:param name="SortOptions" />
  <xsl:param name="Relevancy" />
  <xsl:param name="ModifiedDate" />
  <xsl:param name="DropDownOption" />
  <xsl:param name="Multiply" />
  <xsl:param name="PictureTaken" />
  <xsl:param name="IsNoKeyword" />
  <xsl:param name="IsFixedQuery" />
  <xsl:param name="ShowActionLinks" />
  <xsl:param name="MoreResultsText" />
  <xsl:param name="MoreResultsLink" />
  <xsl:param name="CollapsingStatusLink" />
  <xsl:param name="CollapseDuplicatesText" />
  <xsl:param name="AlertMeLink" />
  <xsl:param name="AlertMeText" />
  <xsl:param name="SrchRSSText" />
  <xsl:param name="SrchRSSLink" />
  <xsl:param name="SearchProviderText" />
  <xsl:param name="SearchProviderLink" />
  <xsl:param name="SearchProviderAlt"/>
  <xsl:param name="ShowMessage" />
  <xsl:param name="IsThisListScope" />
  <xsl:param name="DisplayDiscoveredDefinition" select="True" />
  <xsl:param name="NoFixedQuery" />
  <xsl:param name="NoKeyword" />
  <xsl:param name="ResultsNotFound" />
  <xsl:param name="NoResultsSuggestion" />
  <xsl:param name="NoResultsSuggestion1" />
  <xsl:param name="NoResultsSuggestion2" />
  <xsl:param name="NoResultsSuggestion3" />
  <xsl:param name="NoResultsSuggestion4" />
  <xsl:param name="NoResultsSuggestion5" />
  <xsl:param name="AdditionalResources" />
  <xsl:param name="AdditionalResources1" />
  <xsl:param name="AdditionalResources2" />
  <xsl:param name="IsSearchServer" />
  <xsl:param name="Period" />
  <xsl:param name="SearchHelp" />
  <xsl:param name="Tags" />
  <xsl:param name="Authors" />
  <xsl:param name="Date" />
  <xsl:param name="Size" />
  <xsl:param name="ViewInBrowser" />
  <xsl:param name="DefinitionIntro" />
  <xsl:param name="IdPrefix" />
  <xsl:param name="LangPickerHeading" />
  <xsl:param name="LangPickerNodeSet" />
  <xsl:param name="IsDesignMode">True</xsl:param>

  <!-- When there is keywory to issue the search -->
  <xsl:template name="dvt_1.noKeyword">
    <span class="srch-description2">
      <xsl:choose>
        <xsl:when test="$IsFixedQuery">
          <xsl:value-of select="$NoFixedQuery" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$NoKeyword" />
        </xsl:otherwise>
      </xsl:choose>
    </span>
  </xsl:template>


  <!-- When empty result set is returned from search -->
  <xsl:template name="dvt_1.empty">

    <!-- Unfortunately hiding here as I had problems trying to disable via code: (dvt_1.empty)
     <div class="srch-results">
      <xsl:if test="$AlertMeLink and $ShowActionLinks">
        <span class="srch-alertme" >
          <a href ="{$AlertMeLink}" id="CSR_AM1" title="{$AlertMeText}">
            <img style="vertical-align: middle;" src="/_layouts/images/bell.gif" alt="" border="0"/>
            <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
            <xsl:value-of select="$AlertMeText" />
          </a>
        </span>
      </xsl:if>

      <xsl:if test="string-length($SrchRSSLink) &gt; 0 and $ShowActionLinks">
       <xsl:if test="$AlertMeLink">
          |
        </xsl:if> 
        <a type="application/rss+xml" href ="{$SrchRSSLink}" title="{$SrchRSSText}" id="SRCHRSSL">
          <img style="vertical-align: middle;" border="0" src="/_layouts/images/rss.gif" alt=""/>
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          <xsl:value-of select="$SrchRSSText"/>
        </a>
        <xsl:if test="string-length($SearchProviderLink) &gt; 0">|
          <a href ="{$SearchProviderLink}" title="{$SearchProviderText}" >
            <img style="vertical-align: middle;" border="0" src="/_layouts/images/searchfolder.png" alt=""/>
            <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
            <xsl:value-of select="$SearchProviderText"/>
          </a>
        </xsl:if>
      </xsl:if>
    </div>
          -->

    <div class="srch-results" accesskey="W">
      <span class="srch-description2" id="CSR_NO_RESULTS">
        <p>No search results found.
          <xsl:value-of select="$ResultsNotFound" />
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          <strong>
            <xsl:value-of select="$Keyword" />
          </strong>
          <xsl:value-of select="$Period" />
        </p>
        <h3>
          <xsl:value-of select="$NoResultsSuggestion" />
        </h3>
        <ul>
          <li>
            <xsl:value-of select="$NoResultsSuggestion1" />
          </li>
          <li>
            <xsl:value-of select="$NoResultsSuggestion2" />
          </li>
          <li>
            <xsl:value-of select="$NoResultsSuggestion3" />
          </li>
          <xsl:if test="string-length($NoResultsSuggestion4) &gt; 0">
            <li>
              <xsl:value-of select="$NoResultsSuggestion4" />
            </li>
          </xsl:if>
          <xsl:if test="string-length($NoResultsSuggestion5) &gt; 0">
            <li>
              <xsl:value-of select="$NoResultsSuggestion5" />
            </li>
          </xsl:if>
        </ul>
        <h3>
          <xsl:value-of select="$AdditionalResources" />
        </h3>
        <ul>
          <li>
            <xsl:value-of select="$AdditionalResources1" />
            <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
            <xsl:choose>
              <xsl:when test="string-length($IsSearchServer) &gt; 0">
                <a href="javascript:HelpWindowKey('MSSEndUser_FindContent')" label="$SearchHelp">
                  <xsl:value-of select="$SearchHelp" />
                </a>
              </xsl:when>
              <xsl:otherwise>
                <a href="javascript:HelpWindowKey('WSSEndUser_FindContent')" label="$SearchHelp">
                  <xsl:value-of select="$SearchHelp" />
                </a>
              </xsl:otherwise>
            </xsl:choose>
          </li>
          <li>
            <xsl:value-of select="$AdditionalResources2" />
          </li>
        </ul>



      </span>
    </div>
  </xsl:template>

  <!-- Main body template. Sets the Results view (Relevance or date) options -->
  <xsl:template name="dvt_1.body">
    <xsl:if test="$ShowActionLinks">
      <div class="srch-sort-right2" accesskey="W">
        <xsl:if test="$LangPickerNodeSet and count($LangPickerNodeSet) &gt; 0">
          <xsl:value-of select="$LangPickerHeading"/>
          <select class="srch-dropdown" onchange="window.location.href=this.value" id="langpickerdd">
            <xsl:for-each select="$LangPickerNodeSet">
              <xsl:element name="option">
                <xsl:attribute name="value">
                  <xsl:value-of select="@url"/>
                </xsl:attribute>
                <xsl:if test="@selected = 'true'">
                  <xsl:attribute name="selected">selected</xsl:attribute>
                </xsl:if>
                <xsl:value-of select="@title"/>
              </xsl:element>
            </xsl:for-each>
          </select>
          <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
        </xsl:if>
        <xsl:if test="$ShowDropDown = 'true'">
          <xsl:value-of select="$SortBy" />
          <select id="dropdown" title="{$SortOptions}" onchange="PostToUrl(this.value)" class="srch-dropdown">
            <xsl:if test="$DropDownOption = '0' or $ViewByUrl != ''">
              <xsl:element name="option">
                <xsl:attribute name="value">
                  <xsl:value-of select="$ViewByUrl"/>
                </xsl:attribute>
                <xsl:if test="$DropDownOption = '0'">
                  <xsl:attribute
name="selected">selected</xsl:attribute>
                </xsl:if>
                <xsl:value-of select="$Relevancy"/>
              </xsl:element>
            </xsl:if>
            <xsl:if test="$DropDownOption = '1' or $ViewByUrl != ''">
              <xsl:element name="option">
                <xsl:attribute name="value">
                  <xsl:value-of select="$ViewByUrl"/>
                </xsl:attribute>
                <xsl:if test="$DropDownOption = '1'">
                  <xsl:attribute
name="selected">selected</xsl:attribute>
                </xsl:if>
                <xsl:value-of select="$ModifiedDate"/>
              </xsl:element>
            </xsl:if>
          </select>
        </xsl:if>
        <xsl:if test="$AlertMeLink">
          <xsl:if test="$ShowDropDown = 'true'">
          </xsl:if>
<!-- Unfortunately hiding here as I had problems trying to disable via code: (dvt_1.body)
          <span class="srch-alertme" >
            <a href ="{$AlertMeLink}" id="CSR_AM2" title="{$AlertMeText}">
              <img style="vertical-align: middle;" src="/_layouts/images/bell.gif" alt="" border="0"/>
            </a>
            <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
          </span>
-->          
        </xsl:if>
        <xsl:if test="string-length($SrchRSSLink) &gt; 0">

          <!-- Unfortunately hiding here as I had problems trying to disable via code:          
          <a type="application/rss+xml" href ="{$SrchRSSLink}" title="{$SrchRSSText}" id="SRCHRSSL">
            <img style="vertical-align: middle;" border="0" src="/_layouts/images/rss.gif" alt=""/>
          </a>
          -->
          <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
        </xsl:if>
        <xsl:if test="string-length($SearchProviderLink) &gt; 0">
<!-- Unfortunately hiding here as I had problems trying to disable via code:          
          <a href ="{$SearchProviderLink}" title="{$SearchProviderAlt}" >
            <img style="vertical-align: middle;" border="0" src="/_layouts/images/searchfolder.png" alt=""/>
          </a>
-->
        </xsl:if>
      </div>
    </xsl:if>
    <div class="srch-results" accesskey="W">
      <xsl:apply-templates />
    </div>
    <xsl:call-template name="DisplayMoreResultsAnchor" />
  </xsl:template>
  <!-- This template is called for each result -->
  <xsl:template match="TotalResults">
  </xsl:template>
  <xsl:template match="NumberOfResults">
  </xsl:template>

  <xsl:template match="Result">
    <xsl:variable name="id" select="id"/>
    <xsl:variable name="currentId" select="concat($IdPrefix,$id)"/>
    <xsl:variable name="url" select="url"/>

    <xsl:choose>
      <xsl:when test="string-length(picturethumbnailurl) &gt; 0 and contentclass[. = 'STS_ListItem_PictureLibrary']">
        <div style=" padding-top: 2px; padding-bottom: 2px;">
          <div class="srch-picture1">
            <img src="/_layouts/images/imageresult_16x16.png" />
          </div>
          <div class="srch-picture2">
            <img class="srch-picture" src="{picturethumbnailurl}" alt="" />
          </div>
          <span>
            <ul class="srch-picturetext">
              <li class="srch-Title2 srch-Title5">
                <a href="{$url}" id="{concat('CSR_',$id)}" title="{title}">
                  <xsl:choose>
                    <xsl:when test="hithighlightedproperties/HHTitle[. != '']">
                      <xsl:call-template name="HitHighlighting">
                        <xsl:with-param name="hh" select="hithighlightedproperties/HHTitle" />
                      </xsl:call-template>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="title"/>
                    </xsl:otherwise>
                  </xsl:choose>
                </a>
              </li>

              <li>
                <xsl:if test="string-length(picturewidth) &gt; 0 and string-length(pictureheight) &gt; 0">
                  <xsl:value-of select="$Size" />
                  <xsl:value-of select="picturewidth" />
                  <xsl:value-of select="$Multiply" />
                  <xsl:value-of select="pictureheight" />

                  <xsl:if test="string-length(size) &gt; 0">
                    <xsl:if test="number(size) &gt; 0">
                      <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
                      <xsl:choose>
                        <xsl:when test="round(size div 1024) &lt; 1">
                          <xsl:value-of select="size" /> Bytes
                        </xsl:when>
                        <xsl:when test="round(size div (1024 *1024)) &lt; 1">
                          <xsl:value-of select="round(size div 1024)" />KB
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="round(size div (1024 * 1024))"/>MB
                        </xsl:otherwise>
                      </xsl:choose>
                    </xsl:if>
                  </xsl:if>
                </xsl:if>

                <xsl:if test="string-length(datepicturetaken) &gt; 0">
                  <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
                  <xsl:value-of select="$PictureTaken" />
                  <xsl:value-of select="datepicturetaken" />
                </xsl:if>

                <xsl:if test="string-length(author) &gt; 0">
                  <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
                  <xsl:value-of select="$Authors" />
                  <xsl:value-of select="author" />
                </xsl:if>

                <xsl:if test="string-length(write) &gt; 0">
                  <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
                  <xsl:value-of select="$Date" />
                  <xsl:value-of select="write" />
                </xsl:if>

              </li>

              <li>
                <span class="srch-URL2" id="{concat($currentId,'_Url')}">
                  <xsl:choose>
                    <xsl:when test="hithighlightedproperties/HHUrl[. != '']">
                      <xsl:call-template name="HitHighlighting">
                        <xsl:with-param name="hh" select="hithighlightedproperties/HHUrl" />
                      </xsl:call-template>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="url"/>
                    </xsl:otherwise>
                  </xsl:choose>

                </span>
              </li>
            </ul>
          </span>
        </div>
        <div class="srch-clear">
          <img alt="" src="/_layouts/images/blank.gif" />
        </div>
      </xsl:when>
      <xsl:otherwise>
        <div class="srch-Icon" id="{concat($currentId,'_Icon')}">

          <xsl:choose>
            <xsl:when test="(contentclass = 'STS_Web') and (contains(spweburl, 'izzi/teams'))">
              <img align="absmiddle" src="/_layouts/images/WorkBoxFramework/team-16.png" border="0" alt="Team site for {sitetitle}" />
            </xsl:when>
            <xsl:otherwise>
              <img align="absmiddle" src="{imageurl}" border="0" alt="{imageurl/@imageurldescription}" />
            </xsl:otherwise>
          </xsl:choose>


        </div>
        <div class="srch-Title2">
          <div class="srch-Title3">
            <!-- links with the file scheme only work in ie if they are unescaped. For  
         this reason here we will render the link using disable-output-escaping if the url 
         begins with file.-->
            <xsl:choose>
              <xsl:when test="substring($url,1,5) = 'file:' and $IsDesignMode = 'False'">
                <xsl:text     disable-output-escaping="yes">&lt;a href="</xsl:text>
                <xsl:value-of disable-output-escaping="yes" select="srwrt:HtmlAttributeEncode($url)" />
                <xsl:text     disable-output-escaping="yes">" id="</xsl:text>
                <xsl:value-of disable-output-escaping="yes" select="srwrt:HtmlAttributeEncode(concat($currentId,'_Title'))" />
                <xsl:text     disable-output-escaping="yes">" title="</xsl:text>
                <xsl:value-of disable-output-escaping="yes" select="srwrt:HtmlAttributeEncode(title)" />
                <xsl:text     disable-output-escaping="yes">"&gt;</xsl:text>
                <xsl:choose>
                  <xsl:when test="hithighlightedproperties/HHTitle[. != '']">
                    <xsl:call-template name="HitHighlighting">
                      <xsl:with-param name="hh" select="hithighlightedproperties/HHTitle" />
                    </xsl:call-template>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="srwrt:HtmlEncode(title)"/>
                  </xsl:otherwise>
                </xsl:choose>
                <xsl:text disable-output-escaping="yes">&lt;/a&gt;</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <a id="{concat($currentId,'_Title')}">
                  <xsl:attribute name="href">
                    <xsl:value-of  select="$url"/>
                  </xsl:attribute>
                  <xsl:attribute name="title">
                    <xsl:value-of select="title"/>
                  </xsl:attribute>
                  <xsl:choose>
                    <xsl:when test="hithighlightedproperties/HHTitle[. != '']">
                      <xsl:call-template name="HitHighlighting">
                        <xsl:with-param name="hh" select="hithighlightedproperties/HHTitle" />
                      </xsl:call-template>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="title"/>
                    </xsl:otherwise>
                  </xsl:choose>
                </a>
              </xsl:otherwise>
            </xsl:choose>

            <xsl:if test="(isdocument = 'True') and (owningteam != '') and not(contains(spweburl, 'http://uatizzi'))">
              <span style=" padding-left: 20px; padding-right: 20px; "> in </span>

              <a>
                <xsl:attribute name="href">
                  <xsl:value-of select="spweburl"/>
                </xsl:attribute>

                <img src="/_layouts/images/WorkBoxFramework/work-box-16.png" border="0" alt="Work box icon" style=" padding-right: 5px" />

                <xsl:value-of select="sitetitle"/>
              </a>
            </xsl:if>


          </div>
        </div>

        <div class="srch-Description2">

          <xsl:choose>
            <xsl:when test="hithighlightedsummary[. != '']">
              <xsl:call-template name="HitHighlighting">
                <xsl:with-param name="hh" select="hithighlightedsummary" />
              </xsl:call-template>
            </xsl:when>
            <xsl:when test="description[. != '']">
              <xsl:value-of select="description"/>
            </xsl:when>
            <xsl:otherwise>
              <img alt="" src="/_layouts/images/blank.gif" height="0" width="0"/>
            </xsl:otherwise>
          </xsl:choose>
        </div >


        <div class="srch-Metadata2">
          <xsl:call-template name="DisplayAuthors">
            <xsl:with-param name="author" select="author" />
          </xsl:call-template>
          <xsl:call-template name="DisplayDate">
            <xsl:with-param name="write" select="write" />
          </xsl:call-template>
          <xsl:if test="string-length(popularsocialtag0) &gt; 0">
            <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
            <xsl:value-of select="$Tags" />
            <xsl:value-of select="popularsocialtag0"/>
            <xsl:if test="string-length(popularsocialtag1) &gt; 0">
              ::
              <xsl:value-of select="popularsocialtag1"/>
            </xsl:if>
            <xsl:if test="string-length(popularsocialtag2) &gt; 0">
              ::
              <xsl:value-of select="popularsocialtag2"/>
            </xsl:if>
          </xsl:if>
          <xsl:call-template name="DisplaySize">
            <xsl:with-param name="size" select="size" />
          </xsl:call-template>
          <img style="display:none;" alt="" src="/_layouts/images/blank.gif"/>
        </div>

        <p class="srch-Metadata1">
          <span>
            <span class="srch-URL2" id="{concat($currentId,'_Url')}">

              <xsl:choose>
                <xsl:when test="hithighlightedproperties/HHUrl[. != '']">
                  <xsl:call-template name="HitHighlighting">
                    <xsl:with-param name="hh" select="hithighlightedproperties/HHUrl" />
                  </xsl:call-template>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="url"/>
                </xsl:otherwise>
              </xsl:choose>
            </span>
            <xsl:call-template name="DisplayCollapsingStatusLink">
              <xsl:with-param name="status" select="collapsingstatus"/>
              <xsl:with-param name="workid" select="workid"/>
              <xsl:with-param name="id" select="concat($currentId,'_CS')"/>
            </xsl:call-template>
            <xsl:call-template name="ViewInBrowser">
              <xsl:with-param name="browserlink" select="serverredirectedurl" />
              <xsl:with-param name="currentId" select="$currentId" />
            </xsl:call-template>
          </span>
        </p>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="HitHighlighting">
    <xsl:param name="hh" />
    <xsl:apply-templates select="$hh"/>
  </xsl:template>

  <xsl:template match="ddd">
    &#8230;
  </xsl:template>
  <xsl:template match="c0">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c1">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c2">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c3">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c4">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c5">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c6">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c7">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c8">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>
  <xsl:template match="c9">
    <strong>
      <xsl:value-of select="."/>
    </strong>
  </xsl:template>

  <xsl:template name="DisplayAuthors">
    <xsl:param name="author" />
    <xsl:if test="string-length($author) &gt; 0">
      <xsl:value-of select="$Authors" />
      <xsl:choose>
        <xsl:when test="string-length(author_multival) &gt; 0">
          <xsl:for-each select="author_multival">
            <xsl:variable name="p" select="position()"/>
            <xsl:if test="$p &gt; 1">
              <xsl:text disable-output-escaping="yes">&#44;</xsl:text>
              <xsl:text disable-output-escaping="yes">&#32;</xsl:text>
            </xsl:if>
            <xsl:value-of select="."/>
          </xsl:for-each>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="author"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>

  <xsl:template name="DisplayDate">
    <xsl:param name="write" />
    <xsl:if test="string-length($write) &gt; 0">
      <xsl:if test="string-length(author) &gt; 0">
        <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
      </xsl:if>
      <xsl:value-of select="$Date" />
      <xsl:value-of select="write"/>
    </xsl:if>
  </xsl:template>


  <!-- The size attribute for each result is prepared here -->
  <xsl:template name="DisplaySize">
    <xsl:param name="size" />
    <xsl:if test="string-length($size) &gt; 0">
      <xsl:if test="number($size) &gt; 0">
        <xsl:if test="string-length(write) &gt; 0 or string-length(author) &gt; 0">
          <xsl:text disable-output-escaping="yes">&#8195;</xsl:text>
        </xsl:if>
        <xsl:value-of select="$Size" />
        <xsl:choose>
          <xsl:when test="round($size div 1024) &lt; 1">
            <xsl:value-of select="$size" /> Bytes
          </xsl:when>
          <xsl:when test="round($size div (1024 *1024)) &lt; 1">
            <xsl:value-of select="round($size div 1024)" />KB
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="round($size div (1024 * 1024))"/>MB
          </xsl:otherwise>
        </xsl:choose>
      </xsl:if>
    </xsl:if>
  </xsl:template>

  <xsl:template name="ViewInBrowser">
    <xsl:param name="browserlink" />
    <xsl:param name="currentId" />
    <xsl:if test="string-length($browserlink) &gt; 0">
      <span class="srch-urllink">
        <a href="{$browserlink}" id="{concat($currentId,'_VBlink')}">
          <xsl:value-of select="$ViewInBrowser" />
        </a>
      </span>
    </xsl:if>
  </xsl:template>

  <!-- A generic template to display string with non 0 string length (used for author and lastmodified time -->
  <xsl:template name="DisplayString">
    <xsl:param name="str" />
    <xsl:if test='string-length($str) &gt; 0'>
      -
      <xsl:value-of select="$str" />
    </xsl:if>
  </xsl:template>

  <!-- document collapsing link setup -->
  <xsl:template name="DisplayCollapsingStatusLink">
    <xsl:param name="status"/>
    <xsl:param name="workid"/>
    <xsl:param name="id"/>
    <xsl:if test="$CollapsingStatusLink">
      <xsl:choose>
        <xsl:when test="$status=1">
          <xsl:variable name="CollapsingStatusHref" select="concat(substring-before($CollapsingStatusLink, '$$COLLAPSE_PARAM$$'), 'duplicates:&quot;', $workid, '&quot;', substring-after($CollapsingStatusLink, '$$COLLAPSE_PARAM$$'))"/>
          <span class="srch-urllink">
            <a href="{$CollapsingStatusHref}" id="$id" title="{$CollapseDuplicatesText}">
              <xsl:value-of select="$CollapseDuplicatesText"/>
            </a>
          </span>
        </xsl:when>
      </xsl:choose>
    </xsl:if>
  </xsl:template>
  <!-- The "view more results" for fixed query -->
  <xsl:template name="DisplayMoreResultsAnchor">
    <xsl:if test="$MoreResultsLink">
      <a href="{$MoreResultsLink}" id="{concat($IdPrefix,'_MRL')}">
        <xsl:value-of select="$MoreResultsText"/>
      </a>
    </xsl:if>
  </xsl:template>

  <xsl:template match="All_Results/DiscoveredDefinitions">
    <xsl:variable name="FoundIn" select="DDFoundIn" />
    <xsl:variable name="DDSearchTerm" select="DDSearchTerm" />
    <xsl:if test="$DisplayDiscoveredDefinition = 'True' and string-length($DDSearchTerm) &gt; 0">
      <script language="javascript">
        function ToggleDefinitionSelection()
        {
        var selection = document.getElementById("definitionSelection");
        if (selection.style.display == "none")
        {
        selection.style.display = "inline";
        }
        else
        {
        selection.style.display = "none";
        }
        }
      </script>
      <div class="srch-Description2 srch-definition2">
        <a href="javascript:ToggleDefinitionSelection();" id="{concat($IdPrefix,'1_DEF')}" mss_definition="true">
          <xsl:value-of select="$DefinitionIntro" />
          <strong>
            <xsl:value-of select="$DDSearchTerm"/>
          </strong>
        </a>
        <div id="definitionSelection" class="srch-Description2" style="display:none;">
          <xsl:for-each select="DDefinitions/DDefinition">
            <br/>
            <xsl:variable name="DDUrl" select="DDUrl" />
            <img style="display:inline" alt="" src="/_layouts/images/discovered_definitions_bullet.png" />
            <xsl:value-of select="DDStart"/>
            <strong>
              <xsl:value-of select="DDBold"/>
            </strong>
            <xsl:value-of select="DDEnd"/>
            <br/>
            <span class="srch-definition">
              <xsl:value-of select="$FoundIn"/>
              <xsl:text disable-output-escaping="yes">&#160;</xsl:text>
              <a href="{$DDUrl}">
                <xsl:value-of select="DDTitle"/>
              </a>
            </span>
          </xsl:for-each>
        </div>
      </div>
    </xsl:if>
  </xsl:template>

  <!-- XSL transformation starts here -->
  <xsl:template match="/">
    <xsl:if test="$AlertMeLink">
      <input type="hidden" name="P_Query" />
      <input type="hidden" name="P_LastNotificationTime" />
    </xsl:if>
    <xsl:choose>
      <xsl:when test="$IsNoKeyword = 'True'" >
        <xsl:call-template name="dvt_1.noKeyword" />
      </xsl:when>
      <xsl:when test="$ShowMessage = 'True'">
        <xsl:call-template name="dvt_1.empty" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="dvt_1.body"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- End of Stylesheet -->
</xsl:stylesheet>