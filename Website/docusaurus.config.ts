import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

const config: Config = {
  title: 'PortalLibrarySystem',
  tagline: '～あなたのホームワールドをポータルワールドに～',
  favicon: 'img/icon.png',

  // Future flags, see https://docusaurus.io/docs/api/docusaurus-config#future
  future: {
    v4: true, // Improve compatibility with the upcoming Docusaurus v4
  },

  url: 'https://GenkaiKogyo-ultd.github.io',
  baseUrl: '/PortalLibrarySystem/',
  organizationName: 'GenkaiKogyo-ultd',
  projectName: 'PortalLibrarySystem',
  trailingSlash: false,

  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',

  i18n: {
    defaultLocale: 'jp',
    locales: ['jp'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: './sidebars.ts',
        },
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    navbar: {
      title: '',
      logo: {
        alt: 'PLS Logo',
        src: '/img/logo.png',
      },
      items: [
        {
          type: 'docSidebar',
          sidebarId: 'tutorialSidebar',
          position: 'left',
          label: 'マニュアル',
        },
        {
          href: 'https://booth.pm/ja/items/6659099',
          label: 'BOOTH',
          position: 'right',
        },
        {
          href: 'https://github.com/GenkaiKogyo-ultd/PortalLibrarySystem',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'マニュアル',
          items: [
            {
              label: 'PortalLibrarySystem',
              to: '/docs/PortalLibrarySystem',
            },
            {
              label: 'セットアップ',
              to: '/docs/setup',
            },
            {
              label: 'Ver.1 をお使いの方へ',
              to: '/docs/for_ver1_users',
            },
          ],
        },
        {
          title: 'リンク',
          items: [
            {
              label: '公式ホームページ',
              href: 'https://www.genkaikogyo-ultd.com/%E3%83%9B%E3%83%BC%E3%83%A0',
            },
            {
              label: '幻会コネクト',
              href: 'https://discord.gg/KRFpwrCZ7F',
            },
            {
              label: 'BOOTH',
              href: 'https://booth.pm/ja/items/6659099',
            },
            {
              label: 'GitHub',
              href: 'https://github.com/GenkaiKogyo-ultd/PortalLibrarySystem',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} GenkaiKogyo.uLtd. Built with Docusaurus.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
