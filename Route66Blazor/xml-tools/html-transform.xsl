<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html" omit-xml-declaration="yes"/>

  <xsl:template match="row">
    <div class="row" style="position: static; height: 1.1em; font-family: Consolas, monospace;">
      <xsl:apply-templates />
    </div>
  </xsl:template>

  <xsl:template match="input">
    <input type="text" style="position: absolute; left: {@col}ch; font-family: Consolas, monospace; font-size: 16px" />
  </xsl:template>
  <xsl:template match="label">
    <span class="label" style="position: absolute; left: {@col}ch;">
      <xsl:value-of select="." disable-output-escaping="no"/>
    </span>
  </xsl:template>
</xsl:stylesheet>
