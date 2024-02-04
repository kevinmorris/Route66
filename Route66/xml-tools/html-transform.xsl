<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html" omit-xml-declaration="yes"/>

  <xsl:template match="/">
    <div class="row" style="position: static; height: 1.1em; white-space: pre; font-family: Consolas, monospace;">
      <xsl:for-each select="row/label">
        <span class="label" style="position: absolute; left: {@col}ch;">
          <xsl:value-of select="." disable-output-escaping="yes"/>
        </span>
      </xsl:for-each>
    </div>
  </xsl:template>
</xsl:stylesheet>